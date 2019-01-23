using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Adapters.Handlers.Database;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("db/table")]
        public async Task<ActionResult> DatabaseGetTable([FromBody] GetTable.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        [HttpPost("db/table/fields")]
        public async Task<ActionResult> DatabaseGetTableFields([FromBody] GetTableFields.Request request)
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
                            {
                                {
                                    "AdapterFields", new RequestAdapterNode()
                                    {
                                        Url = "http://localhost:4967/api/adapters/db/table/fields",
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
                                            { "Text", new RequestNode("AdapterFields", "Description") }
                                        }
                                    }
                                },
                                { "Attributes", new RequestNode("AdapterFields", "ForCs.EFAttributes") },
                                { "TypeName", new RequestNode("AdapterFields", "TypeName") },
                                { "PropertyName", new RequestNode("AdapterFields", "Name") }
                            }
                        }
                    }
                }
            };

            await mediator.Send(new RequestNodes.Handlers.CreateRequestNode.Request()
            {
                Node = node
            });


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