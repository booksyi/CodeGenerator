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
        public async Task<object> ToAdapter(AdapterNode adapterNode, Dictionary<string, object> request, Dictionary<string, Dictionary<string, object>> requestAdapters)
        {
            Dictionary<string, object> adapterRequest = new Dictionary<string, object>();
            //List<KeyValuePair<string, string>> adapterRequest = new List<KeyValuePair<string, string>>();
            foreach (var nodeRequest in adapterNode.Request)
            {
                if (nodeRequest.Value.Type == RequestType.Request)
                {
                    IEnumerable<KeyValuePair<string, object>> temp;
                    // get request values from http or adapters
                    if (nodeRequest.Value.Key.Contains("->"))
                    {
                        string name = nodeRequest.Value.Key.Remove(nodeRequest.Value.Key.IndexOf("->"));
                        string key = nodeRequest.Value.Key.Substring(nodeRequest.Value.Key.IndexOf("->") + "->".Length);
                        temp = requestAdapters[name]
                            .Where(x => x.Key == key)
                            .Select(x => new KeyValuePair<string, object>(nodeRequest.Key, x.Value)));
                    }
                    else
                    {
                        temp = request
                            .Where(x => x.Key == nodeRequest.Value.Key)
                            .Select(x => new KeyValuePair<string, object>(nodeRequest.Key, x.Value)));
                    }
                    // unification or separation
                    if (nodeRequest.Value.Mode == GenerateMode.Unification)
                    {
                        adapterRequest = temp.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Select(y => y.Value).ToArray() as object);
                    }
                    else if (nodeRequest.Value.Mode == GenerateMode.Separation)
                    {
                        // TODO:
                    }
                }
            }

            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync(adapterNode.Url, adapterRequest);
            var json = await response.Content.ReadAsStringAsync();
            if (json.Trim().StartsWith("[")
                && json.Trim().EndsWith("]"))
            {
                return JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(json);
            }
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public async Task<IEnumerable<GenerateNode>> ToGenerateNode(RequestNode requestNode, Dictionary<string, object> request, Dictionary<string, Dictionary<string, object>> requestAdapters)
        {
            if (adapters == null)
            {
                adapters = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
            }

            if (requestNode.AdapterNodes != null)
            {
                foreach (var adapterNode in requestNode.AdapterNodes)
                {
                    adapters.Add(adapterNode.Key, await GetAdapter(adapterNode.Value, adapters, request));
                }
            }

            List<GenerateNode> generateNodes = new List<GenerateNode>();
            GenerateNode generateNode = new GenerateNode();
            if (root.Type == RequestType.Template)
            {
                generateNode.ApplyFilePath = $@"D:\Workspace\CodeGenerator\CodeGenerator\Templates\CSharp\{root.Key}.html";
                foreach (var a in root.Parameters)
                {
                    var temps = await ToGenerateNode(a.Value, adapters, request);
                    foreach (var b in temps)
                    {
                        generateNode.AppendChild(b).ChangeKey(a.Key);
                    }
                }
            }
            else if (root.Type == RequestType.Request)
            {
                if (root.Key.Contains("->"))
                {
                    string name = root.Key.Remove(root.Key.IndexOf("->"));
                    string key = root.Key.Substring(root.Key.IndexOf("->") + "->".Length);
                    return adapters[name].Where(x => x.Key == key).Select(x => new GenerateNode() { ApplyValue = x.Value }).ToArray();
                }
                else
                {
                    return request.Where(x => x.Key == root.Key).Select(x => new GenerateNode() { ApplyValue = x.Value }).ToArray();
                }
            }
            generateNodes.Add(generateNode);
            return generateNodes;
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

        [HttpPost("[action]")]
        public async Task<ActionResult> Test([FromBody] Dictionary<string, string[]> rr)
        {
            RequestNode node = new RequestNode()
            {
                Type = RequestType.Template,
                Key = "EntityFrameworkModel",
                Parameters = new Dictionary<string, RequestNode>
                {
                    { "ProjectName", new RequestNode("projectName") },
                    { "TableName", new RequestNode("tableName") },
                    { "ModelName", new RequestNode("modelName") },
                    {
                        "Properties", new RequestNode()
                        {
                            Type = RequestType.Template,
                            Key = "DeclareProperty",
                            Adapters = new Dictionary<string, Adapter>
                            {
                                {
                                    "Adapter1", new Adapter()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFieldNames",
                                        Request = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") },
                                        }
                                    }
                                },
                                {
                                    "Adapter2", new Adapter()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFieldInfo",
                                        Request = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") },
                                            { "fieldName", new RequestNode("Adapter1->FieldNames") { Mode = GenerateMode.Separation } },
                                        }
                                    }
                                }
                            },
                            Parameters = new Dictionary<string, RequestNode>
                            {
                                {
                                    "Summary", new RequestNode()
                                    {
                                        Type = RequestType.Template,
                                        Key = "Summary",
                                        Parameters = new Dictionary<string, RequestNode>
                                        {
                                            { "Text", new RequestNode("Adapter2->Description") }
                                        }
                                    }
                                },
                                { "Attributes", new RequestNode("Adapter2->Attributes") },
                                { "TypeName", new RequestNode("Adapter2->TypeName") },
                                { "PropertyName", new RequestNode("Adapter2->PropertyName") }
                            }
                        }
                    }
                }
            };

            List<KeyValuePair<string, string>> request = new List<KeyValuePair<string, string>>();

            foreach (var a in rr)
            {
                foreach (var b in a.Value)
                {
                    request.Add(new KeyValuePair<string, string>(a.Key, b));
                }
            }

            QQ qQ = new QQ();
            //GenerateNode generateNode = await qQ.ToGenerateNode(node, null, request);
            GenerateNode generateNode = (await qQ.ToGenerateNode(node, null, request)).FirstOrDefault();
            return new OkObjectResult(generateNode);
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