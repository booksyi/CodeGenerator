﻿using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Testers.Handlers.TestCases
{
    [Description("測試 Adapter 串接 TemplateNode 的 Url 參數")]
    public class Test4
    {
        public class Request : IRequest<uint>
        {
        }

        // $"{host}/api/testers/templates/{Tester}/{Template}"
        public class Templates
        {
            public static string Template1(JObject request)
            {
                return @"{{# Fields, @@Space }}";
            }

            public static string Template2(JObject request)
            {
                string description = Convert.ToString(request["Description"]);
                if (string.IsNullOrWhiteSpace(description))
                {
                    return @"public {{# Name }} { get; set; }";
                }
                return @"public {{# Name }} { get; set; } /* {{# Description }} */";
            }
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
        public class Adapters
        {
            public static JToken Adapter1(JObject request)
            {
                return JToken.FromObject(new
                {
                    Fields = new object[]
                    {
                        new { Name = "AA", Description = "標題" },
                        new { Name = "BB", Description = "內文" },
                        new { Name = "CC", Description = "" },
                    }
                });
            }
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
                string tester = "Test4";
                string host = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
                JObject httpRequest = new JObject()
                {
                };

                CodeTemplate template = new CodeTemplate()
                {
                    TemplateNodes = new CodeTemplate.TemplateNode[]
                    {
                        new CodeTemplate.TemplateNode()
                        {
                            Url = $"{host}/api/testers/templates/{tester}/Template1",
                            ParameterNodes = new CodeTemplate.ParameterNode[]
                            {
                                new CodeTemplate.ParameterNode("Fields")
                                {
                                    From = CodeTemplate.ParameterFrom.Template,
                                    TemplateNode = new CodeTemplate.TemplateNode()
                                    {
                                        Url = $"{host}/api/testers/templates/{tester}/Template2",
                                        RequestNodes = new CodeTemplate.RequestNode[]
                                        {
                                            new CodeTemplate.RequestNode("Description").FromAdapter("Adapter1", "Fields.Description")
                                        },
                                        AdapterNodes = new CodeTemplate.AdapterNode[]
                                        {
                                            new CodeTemplate.AdapterNode(
                                                "Adapter1",
                                                $"{host}/api/testers/adapters/{tester}/Adapter1")
                                            {
                                                IsSplit = true
                                            }
                                        },
                                        ParameterNodes = new CodeTemplate.ParameterNode[]
                                        {
                                            new CodeTemplate.ParameterNode("Name").FromAdapter("Adapter1", "Fields.Name"),
                                            new CodeTemplate.ParameterNode("Description").FromAdapter("Adapter1", "Fields.Description")
                                        }
                                    }
                                }
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

                return result[0] == "public AA { get; set; } /* 標題 */ public BB { get; set; } /* 內文 */ public CC { get; set; }";
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
