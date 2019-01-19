using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CodeGenerator.Controllers.Adapters.Actions;
using CodeGenerator.Data.Models;
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
                        var expandNodes = await Expand(requestNode.Parameters[key]);
                        requestNode.ExpandParameters.Add(key, expandNodes.ToArray());
                    }
                    else
                    {
                        requestNode.ExpandParameters.Add(key, new RequestNode[] { requestNode.Parameters[key] });
                    }
                }
            }
            #endregion
            return new RequestNode[] { requestNode };
        }

        public async Task<IEnumerable<GenerateNode>> ToGenerateNode(RequestNode requestNode)
        {
            if (requestNode.From == RequestFrom.Template)
            {
                GenerateNode generateNode = new GenerateNode();
                generateNode.ApplyApi = $@"https://codegeneratoradapters.azurewebsites.net/api/GetTemplate?code=jzG4qdc0Lo3Hp5TiPkFiaRoMlDXGHQNWCmNrr59KZFTabesbOAgJUg==&name={requestNode.Key}";
                foreach (var parameter in requestNode.ExpandParameters)
                {
                    foreach (var node in parameter.Value)
                    {
                        var children = await ToGenerateNode(node);
                        foreach (var child in children)
                        {
                            generateNode
                                .AppendChild(child)
                                .ChangeKey(parameter.Key);
                        }
                    }
                }
                return new GenerateNode[] { generateNode };
            }
            else if (requestNode.From == RequestFrom.Request)
            {
                List<GenerateNode> generateNodes = new List<GenerateNode>();
                var values = requestNode.Request
                    .Where(x => x.Key == requestNode.Key)
                    .Select(x => Convert.ToString(x.Value)).ToArray();
                foreach (var value in values)
                {
                    generateNodes.Add(new GenerateNode(requestNode.Key, value));
                }
                return generateNodes;
            }
            else if (requestNode.From == RequestFrom.Adapter)
            {
                List<GenerateNode> generateNodes = new List<GenerateNode>();
                var value =
                    (requestNode.Adapters[requestNode.AdapterName] as JObject)?
                    .Property(requestNode.Key, StringComparison.CurrentCultureIgnoreCase)?
                    .Value;
                if (value != null)
                {
                    if (value is JValue jValue)
                    {
                        generateNodes.Add(new GenerateNode(
                            requestNode.Key,
                            Convert.ToString(jValue)));
                    }
                    else if (value is JObject jObject)
                    {
                        generateNodes.Add(new GenerateNode(
                            requestNode.Key,
                            Convert.ToString(jObject)));
                    }
                    else if (value is JArray jArray)
                    {
                        foreach (var jv in jArray)
                        {
                            generateNodes.Add(new GenerateNode(
                                requestNode.Key,
                                Convert.ToString(jv)));
                        }
                    }
                }
                return generateNodes;
            }
            return new GenerateNode[0];
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
            node.Request = rr;

            QQ qQ = new QQ();
            qQ.SetRequest(node);
            RequestNode[] nodes = (await qQ.Expand(node)).ToArray();
            List<GenerateNode> generateNodes = new List<GenerateNode>();
            foreach (var rnode in nodes)
            {
                generateNodes.AddRange(await qQ.ToGenerateNode(rnode));
            }
            return new OkObjectResult(generateNodes);
        }
    }
}