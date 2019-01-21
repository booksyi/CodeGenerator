using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CodeGenerator.Controllers.Adapters.Actions;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pluralize.NET.Core;

namespace CodeGenerator.Controllers.Adapters
{
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
        public async Task<ActionResult> DbTableFields([FromBody] DbTableFields.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        // /api/Adapters/Test
        [HttpPost("[action]")]
        public async Task<ActionResult> Test([FromBody] Dictionary<string, JToken> rr)
        {
            RequestNode node = new RequestNode()
            {
                From = RequestFrom.Template,
                TemplateUrl = "https://codegeneratoradapters.azurewebsites.net/api/GetTemplate?code=jzG4qdc0Lo3Hp5TiPkFiaRoMlDXGHQNWCmNrr59KZFTabesbOAgJUg==&name=EntityFrameworkModel",
                TemplateRequestNodes = new Dictionary<string, RequestNode>
                {
                    { "ProjectName", new RequestNode("projectName") },
                    { "TableName", new RequestNode("tableName") },
                    { "ModelName", new RequestNode("modelName") },
                    {
                        "Properties", new RequestNode()
                        {
                            From = RequestFrom.Template,
                            TemplateUrl = "https://codegeneratoradapters.azurewebsites.net/api/GetTemplate?code=jzG4qdc0Lo3Hp5TiPkFiaRoMlDXGHQNWCmNrr59KZFTabesbOAgJUg==&name=DeclareProperty",
                            AdapterNodes = new Dictionary<string, RequestAdapterNode>
                            {/*
                                {
                                    "Adapter1", new RequestAdapterNode()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFieldNames",
                                        RequestNodes = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") },
                                        },
                                        Type = RequestAdapterType.Separation
                                    }
                                },
                                {
                                    "Adapter2", new RequestAdapterNode()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFieldInfo",
                                        RequestNodes = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") },
                                            { "fieldName", new RequestNode("Adapter1", "FieldNames") },
                                        }
                                    }
                                }*/
                                {
                                    "Adapter2", new RequestAdapterNode()
                                    {
                                        Url = "http://localhost:4967/api/Adapters/DbTableFields",
                                        RequestNodes = new Dictionary<string, RequestNode>
                                        {
                                            { "connectionString", new RequestNode("connectionString") },
                                            { "tableName", new RequestNode("tableName") }
                                        },
                                        Type = RequestAdapterType.Separation
                                    }
                                }
                            },
                            TemplateRequestNodes = new Dictionary<string, RequestNode>
                            {
                                {
                                    "Summary", new RequestNode()
                                    {
                                        From = RequestFrom.Template,
                                        TemplateUrl = "https://codegeneratoradapters.azurewebsites.net/api/GetTemplate?code=jzG4qdc0Lo3Hp5TiPkFiaRoMlDXGHQNWCmNrr59KZFTabesbOAgJUg==&name=Summary",
                                        TemplateRequestNodes = new Dictionary<string, RequestNode>
                                        {
                                            { "Text", new RequestNode("Adapter2", "Fields.Description") }
                                        }
                                    }
                                },
                                { "Attributes", new RequestNode("Adapter2", "Fields.ForCs.EFAttributes") },
                                { "TypeName", new RequestNode("Adapter2", "Fields.TypeName") },
                                { "PropertyName", new RequestNode("Adapter2", "Fields.Name") }
                            }
                        }
                    }
                }
            };
            node.HttpRequest = rr;

            node.Deep();
            RequestNode[] nodes = (await node.BuildComplex()).ToArray();
            List<GenerateNode> generateNodes = new List<GenerateNode>();
            foreach (var rnode in nodes)
            {
                generateNodes.AddRange(await rnode.ToGenerateNode());
            }
            return new OkObjectResult(generateNodes);
        }
    }
}