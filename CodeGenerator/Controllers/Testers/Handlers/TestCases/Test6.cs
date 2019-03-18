using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Testers.Handlers.TestCases
{
    /// <summary>
    /// 測試 Input 的物件型態與 Split
    /// </summary>
    public class Test6
    {
        public class Request : IRequest<uint>
        {
        }

        // $"{host}/api/testers/templates/{Tester}/{Template}"
        public class Templates
        {
            public static string Template1(JObject request)
            {
                return @"{{# Prefix }},{{# TableName }},{{# ModelName }}";
            }
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
        public class Adapters
        {
        }

        public class Handler : IRequestHandler<Request, uint>
        {
            private readonly IHttpContextAccessor httpContextAccessor;
            public Handler(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Test(Request request)
            {
                string tester = "Test6";
                string host = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
                JObject httpRequest = new JObject()
                {
                    { "Prefix", JToken.FromObject(new string[] { "A", "B" }) },
                    { "Tables", JToken.FromObject(
                        new JObject[]
                        {
                            new JObject() { { "TableName", "Table1" }, { "ModelName", "Model1" } },
                            new JObject() { { "TableName", "Table2" }, { "ModelName", "Model2" } }
                        })
                    }
                };

                CodeTemplate template = new CodeTemplate()
                {
                    Inputs = new CodeTemplate.Input[]
                    {
                        new CodeTemplate.Input("Prefix").Multiple().Split(),
                        new CodeTemplate.Input("Tables")
                        {
                            Children = new CodeTemplate.InputChild[]
                            {
                                new CodeTemplate.InputChild("TableName"),
                                new CodeTemplate.InputChild("ModelName")
                            }
                        }.Multiple().Split()
                    },
                    TemplateNodes = new CodeTemplate.TemplateNode[]
                    {
                        new CodeTemplate.TemplateNode()
                        {
                            Url = $"{host}/api/testers/templates/{tester}/Template1",
                            ParameterNodes = new CodeTemplate.ParameterNode[]
                            {
                                new CodeTemplate.ParameterNode("Prefix").FromInput("Prefix"),
                                new CodeTemplate.ParameterNode("TableName").FromInput("Tables.TableName"),
                                new CodeTemplate.ParameterNode("ModelName").FromInput("Tables.ModelName")
                            }
                        }
                    }
                };

                List<string> result = new List<string>();
                var generateNodes = await template.ToGenerateNodesAsync(httpRequest);
                foreach (var generateNode in generateNodes)
                {
                    result.Add(await generateNode.GenerateAsync());
                }

                return result.Count == 4
                    && result[0] == "A,Table1,Model1"
                    && result[1] == "A,Table2,Model2"
                    && result[2] == "B,Table1,Model1"
                    && result[3] == "B,Table2,Model2";
            }

            public async Task<uint> Handle(Request request, CancellationToken token)
            {
                if (await Test(request) == false)
                {
                    throw new Exception("測試結果不如預期");
                }
                return 0;
            }
        }
    }
}
