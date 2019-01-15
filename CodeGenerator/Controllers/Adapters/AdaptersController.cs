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
        public async Task<IEnumerable<KeyValuePair<string, string>>> GetAdapter(Adapter adapter, Dictionary<string, IEnumerable<KeyValuePair<string, string>>> adapters, IEnumerable<KeyValuePair<string, string>> request)
        {
            List<KeyValuePair<string, string>> adapterRequest = new List<KeyValuePair<string, string>>();
            foreach (var a in adapter.Request)
            {
                if (a.Value.Type == RequestType.Request)
                {
                    if (a.Value.Key.Contains("->"))
                    {
                        string name = a.Value.Key.Remove(a.Value.Key.IndexOf("->"));
                        string key = a.Value.Key.Substring(a.Value.Key.IndexOf("->") + "->".Length);
                        adapterRequest.AddRange(adapters[name]
                            .Where(x => x.Key == key)
                            .Select(x => new KeyValuePair<string, string>(a.Key, x.Value)));
                    }
                    else
                    {
                        adapterRequest.AddRange(request
                            .Where(x => x.Key == a.Value.Key)
                            .Select(x => new KeyValuePair<string, string>(a.Key, x.Value)));
                    }
                }
            }

            Dictionary<string, object> dic = adapterRequest.GroupBy(x => x.Key).ToDictionary(x => x.Key, x =>
            {
                if (x.Count() == 1 || x.Key == "fieldName")
                {
                    return x.First().Value as object;
                }
                return x.Select(y => y.Value).ToArray() as object;
            });

            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync(adapter.Url, dic);
            var json = await response.Content.ReadAsStringAsync();
            IEnumerable<KeyValuePair<string, string>> result = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(json);
            return result;
        }

        public async Task<IEnumerable<GenerateNode>> ToGenerateNode(RequestNode root, Dictionary<string, IEnumerable<KeyValuePair<string, string>>> adapters, IEnumerable<KeyValuePair<string, string>> request)
        {
            if (adapters == null)
            {
                adapters = new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>();
            }

            if (root.Adapters != null)
            {
                foreach (var a in root.Adapters)
                {
                    adapters.Add(a.Key, await GetAdapter(a.Value, adapters, request));
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