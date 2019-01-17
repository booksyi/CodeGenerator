using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CodeGenerator.Controllers.Adapters.Actions;
using CodeGenerator.Data.Models;
using CodeGenerator.Handlers;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pluralize.NET.Core;

namespace CodeGenerator.Controllers.Adapters
{
    public class QQ
    {
        public void SetRequest(RequestNode node)
        {
            foreach (var adapterNode in node.AdapterNodes)
            {
                foreach (var adapterRequestNode in adapterNode.Value.Request)
                {
                    adapterRequestNode.Value.Request = node.Request;
                }
            }
            if (node.Parameters != null)
            {
                foreach (var parameter in node.Parameters)
                {
                    parameter.Value.Request = node.Request;
                    SetRequest(parameter.Value);
                }
            }
        }

        public void SetAdapter(RequestNode node)
        {
            foreach (var adapterNode in node.AdapterNodes)
            {
                foreach (var adapterRequestNode in adapterNode.Value.Request)
                {
                    adapterRequestNode.Value.Adapters = node.Adapters;
                }
            }
            if (node.Parameters != null)
            {
                foreach (var parameter in node.Parameters)
                {
                    parameter.Value.Adapters = node.Adapters;
                    SetAdapter(parameter.Value);
                }
            }
        }

        public JObject ToHttpRequest(Dictionary<string, RequestNode> nodes)
        {
            JObject jObject = new JObject();
            foreach (var node in nodes)
            {
                object value = null;
                if (node.Value.From == RequestFrom.Request)
                {
                    value = node.Value.Request[node.Value.Key];
                }
                else if (node.Value.From == RequestFrom.Adapter)
                {
                    value = (node.Value.Adapters[node.Value.AdapterName] as JObject).Property(node.Value.Key, StringComparison.CurrentCultureIgnoreCase).Value;
                }
                jObject.Add(node.Key, JToken.FromObject(value));
            }
            return jObject;
        }

        public async Task<JToken> ToAdapter(AdapterNode node)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync(node.Url, ToHttpRequest(node.Request));
            var json = await response.Content.ReadAsStringAsync();
            return JToken.FromObject(JsonConvert.DeserializeObject(json));
        }

        public async Task<IEnumerable<RequestNode>> Expand(RequestNode requestNode)
        {
            #region requestNode.AdapterNodes to Adapters
            if (requestNode.AdapterNodes.Any())
            {
                var adapterNode = requestNode.AdapterNodes.First();
                requestNode.AdapterNodes.Remove(adapterNode.Key);
                JToken adapterValue = await ToAdapter(adapterNode.Value);
                if (adapterValue is JObject jObject)
                {
                    if (adapterNode.Value.Type == AdapterType.Separation)
                    {
                        if (jObject.Properties().Count() == 1 && jObject.Properties().First().Value is JArray jArray)
                        {
                            List<RequestNode> nodes = new List<RequestNode>();
                            foreach (var value in jArray)
                            {
                                JObject jValue = new JObject();
                                jValue.Add(jObject.Properties().First().Name, value);
                                RequestNode clone = requestNode.JsonConvertTo<RequestNode>();
                                clone.Adapters.Add(adapterNode.Key, jValue);
                                SetAdapter(clone);
                                nodes.AddRange(await Expand(clone));
                            }
                            return nodes;
                        }
                    }
                    requestNode.Adapters.Add(adapterNode.Key, jObject);
                    SetAdapter(requestNode);
                    return await Expand(requestNode);
                }
                else if (adapterValue is JArray values)
                {
                    if (adapterNode.Value.Type == AdapterType.Unification)
                    {
                        // TODO: FIX
                        JArray jArray = new JArray();
                        foreach (JToken value in values)
                        {
                            jArray.Add(value);
                        }
                        requestNode.Adapters.Add(adapterNode.Key, jArray);
                        SetAdapter(requestNode);
                        return await Expand(requestNode);
                    }
                    else if (adapterNode.Value.Type == AdapterType.Separation)
                    {
                        List<RequestNode> nodes = new List<RequestNode>();
                        foreach (var value in values)
                        {
                            RequestNode clone = requestNode.JsonConvertTo<RequestNode>();
                            clone.Adapters.Add(adapterNode.Key, value);
                            SetAdapter(clone);
                            nodes.AddRange(await Expand(clone));
                        }
                        return nodes;
                    }
                }
            }
            #endregion

            #region Deep
            if (requestNode.Parameters.Any())
            {
                string[] keys = requestNode.Parameters.Keys.ToArray();
                foreach (var key in keys)
                {
                    if (requestNode.Parameters[key].From == RequestFrom.Template)
                    {
                        // TODO: remove first, 向外擴展
                        requestNode.Parameters[key] = (await Expand(requestNode.Parameters[key])).FirstOrDefault();
                    }
                }
            }
            #endregion
            return new RequestNode[] { requestNode };
        }

        public async Task<GenerateNode> ToGenerateNode(RequestNode requestNode)
        {
            GenerateNode generateNode = new GenerateNode();
            if (requestNode.From == RequestFrom.Template)
            {
                generateNode.ApplyFilePath = $@"D:\Workspace\CodeGenerator\CodeGenerator\Templates\CSharp\{requestNode.Key}.html";
                foreach (var parameter in requestNode.Parameters)
                {
                    generateNode
                        .AppendChild(await ToGenerateNode(parameter.Value))
                        .ChangeKey(parameter.Key);
                }
            }
            else if (requestNode.From == RequestFrom.Request)
            {
                generateNode.AppendChild(
                    requestNode.Key,
                    requestNode.Request
                        .Where(x => x.Key == requestNode.Key)
                        .Select(x => Convert.ToString(x.Value)));
            }
            else if (requestNode.From == RequestFrom.Adapter)
            {
                if (requestNode.Adapters[requestNode.AdapterName] is JObject jObject)
                {
                    generateNode.AppendChild(
                        requestNode.Key,
                        Convert.ToString(jObject));
                }
                else if (requestNode.Adapters[requestNode.AdapterName] is JArray jArray)
                {
                    generateNode.AppendChild(
                        requestNode.Key,
                        jArray.Select(x => Convert.ToString(x)));
                }
            }
            return generateNode;
        }
    }


    [Route("api/[controller]")]
    public class AdaptersController : Controller
    {
        private readonly IMediator mediator;
        private readonly Pluralizer pluralizer;

        public AdaptersController(IMediator mediator, Pluralizer pluralizer)
        {
            this.mediator = mediator;
            this.pluralizer = pluralizer;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> DbTableNames([FromBody] DbTableNames.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> DbTableInfo([FromBody] DbTableInfo.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        // api/Adapters/DbTableFieldNames
        [HttpPost("[action]")]
        public async Task<ActionResult> DbTableFieldNames([FromBody] DbTableFieldNames.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> DbTableFieldInfo([FromBody] DbTableFieldInfo.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        // /api/Adapters/Test
        [HttpPost("[action]")]
        public async Task<ActionResult> Test([FromBody] Dictionary<string, object> rr)
        {
            RequestNode node = new RequestNode()
            {
                From = RequestFrom.Template,
                Key = "EntityFrameworkModel",
                Parameters = new Dictionary<string, RequestNode>
                {
                    { "ProjectName", new RequestNode("projectName") },
                    { "TableName", new RequestNode("tableName") },
                    { "ModelName", new RequestNode("modelName") },
                    {
                        "Properties", new RequestNode()
                        {
                            From = RequestFrom.Template,
                            Key = "DeclareProperty",
                            AdapterNodes = new Dictionary<string, AdapterNode>
                            {
                                {
                                    "Adapter1", new AdapterNode()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFieldNames",
                                        Request = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") },
                                        },
                                        Type = AdapterType.Separation
                                    }
                                },
                                {
                                    "Adapter2", new AdapterNode()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFieldInfo",
                                        Request = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") },
                                            { "fieldName", new RequestNode("Adapter1", "FieldNames") },
                                        }
                                    }
                                }
                            },
                            Parameters = new Dictionary<string, RequestNode>
                            {
                                {
                                    "Summary", new RequestNode()
                                    {
                                        From = RequestFrom.Template,
                                        Key = "Summary",
                                        Parameters = new Dictionary<string, RequestNode>
                                        {
                                            { "Text", new RequestNode("Adapter2", "Description") }
                                        }
                                    }
                                },
                                { "Attributes", new RequestNode("Adapter2", "Attributes") },
                                { "TypeName", new RequestNode("Adapter2", "TypeName") },
                                { "PropertyName", new RequestNode("Adapter2", "PropertyName") }
                            }
                        }
                    }
                }
            };

            /*
            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>();

            foreach (var a in rr)
            {
                foreach (var b in a.Value)
                {
                    request.Add(new KeyValuePair<string, string>(a.Key, b));
                }
            }
            */

            node.Request = rr;
            


            QQ qQ = new QQ();
            qQ.SetRequest(node);
            RequestNode[] nodes = (await qQ.Expand(node)).ToArray();
            GenerateNode[] generateNodes = await Task.WhenAll(nodes.Select(async x => await qQ.ToGenerateNode(x)));
            return new OkObjectResult(generateNodes);
        }

        /*
        [HttpPost("[action]")]
        public async Task<ActionResult> GetNode(string path, [FromBody] IEnumerable<KeyValuePair<string, string>> values)
        {
            GenerateNode node = new GenerateNode();
            node.ApplyFilePath = path;
            foreach (var value in values)
            {
                node.AppendChild(value.Key, value.Value);
            }
            return new OkObjectResult(node);
        }
        */
        /*
        // /api/Adapters/Adapter2
        [HttpPost("[action]")]
        public async Task<ActionResult> Adapter2(string connectionString, string tableName)
        {
            DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(connectionString, tableName);

            List<GenerateNode> nodes = new List<GenerateNode>();
            foreach (var field in tableSchema.Fields)
            {
                GenerateNode node = new GenerateNode() { ApplyFilePath = @"Templates\CSharp\DeclareProperty.html" };
                if (string.IsNullOrWhiteSpace(field.Description) == false)
                {
                    node.AppendChild(await mediator.Send(
                        new GenerateModelPropertySummary.Request()
                        {
                            Text = field.Description
                        })).ChangeKey("Summary");
                }
                node.AppendChild("Attributes", field.ForCs.EFAttributes);
                node.AppendChild("TypeName", field.ForCs.TypeName);
                node.AppendChild("PropertyName", field.Name);

                nodes.Add(node);
            }

            return new OkObjectResult(nodes);
        }
        */
    }
}