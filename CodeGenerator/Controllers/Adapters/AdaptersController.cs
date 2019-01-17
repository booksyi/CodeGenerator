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


        public Dictionary<string, object> ToHttpRequest(Dictionary<string, RequestNode> nodes)
        {
            return nodes.ToDictionary(
                x => x.Key,
                x =>
                {
                    if (x.Value.From == RequestFrom.Request)
                    {
                        return x.Value.Request[x.Value.Key];
                    }
                    else if (x.Value.From == RequestFrom.Adapter)
                    {
                        return x.Value.Adapters[x.Value.AdapterName][x.Value.Key];
                    }
                    return null;
                });
        }

        public async Task<object> ToAdapter(AdapterNode node)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync(node.Url, ToHttpRequest(node.Request));
            var json = await response.Content.ReadAsStringAsync();
            if (json.Trim().StartsWith("[")
                && json.Trim().EndsWith("]"))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>[]>(json);
            }
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public async Task<IEnumerable<RequestNode>> Expand(RequestNode requestNode)
        {
            #region requestNode.AdapterNodes to Adapters
            if (requestNode.AdapterNodes.Any())
            {
                var adapterNode = requestNode.AdapterNodes.First();
                requestNode.AdapterNodes.Remove(adapterNode.Key);
                object adapterValue = await ToAdapter(adapterNode.Value);
                if (adapterValue is Dictionary<string, object> value)
                {
                    if (adapterNode.Value.Type == AdapterType.Separation)
                    {
                        if (value.Count == 1 && value.First().Value.GetType().IsArray)
                        {
                            List<RequestNode> nodes = new List<RequestNode>();
                            Dictionary<string, object>[] innerValues = value.First().Value.JsonConvertTo<Dictionary<string, object>[]>();
                            foreach (var _value in innerValues)
                            {
                                RequestNode clone = requestNode.JsonConvertTo<RequestNode>();
                                clone.Adapters.Add(adapterNode.Key, _value);
                                SetAdapter(clone);
                                nodes.AddRange(await Expand(clone));
                            }
                            return nodes;
                        }
                    }
                    requestNode.Adapters.Add(adapterNode.Key, value);
                    SetAdapter(requestNode);
                    return await Expand(requestNode);
                }
                else if (adapterValue is Dictionary<string, object>[] values)
                {
                    if (adapterNode.Value.Type == AdapterType.Unification)
                    {
                        requestNode.Adapters.Add(
                            adapterNode.Key,
                            values
                                .SelectMany(x => x)
                                .GroupBy(x => x.Key)
                                .ToDictionary(
                                    x => x.Key,
                                    x => x.Select(y => y.Value) as object));
                        SetAdapter(requestNode);
                        return await Expand(requestNode);
                    }
                    else if (adapterNode.Value.Type == AdapterType.Separation)
                    {
                        List<RequestNode> nodes = new List<RequestNode>();
                        foreach (var _value in values)
                        {
                            RequestNode clone = requestNode.JsonConvertTo<RequestNode>();
                            clone.Adapters.Add(adapterNode.Key, _value);
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
                generateNode.AppendChild(
                    requestNode.Key,
                    requestNode.Adapters[requestNode.AdapterName]
                        .Where(x => x.Key == requestNode.AdapterName)
                        .Select(x => Convert.ToString(x.Value)));
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