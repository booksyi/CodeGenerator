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
    /// 測試多層的 Adapter
    /// </summary>
    public class Test5
    {
        public class Request : IRequest<uint>
        {
        }

        // $"{host}/api/testers/templates/{Tester}/{Template}"
        public class Templates
        {
            public static string Template1(JObject request)
            {
                return @"{{# Result }}";
            }
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
        public class Adapters
        {
            public static JToken Adapter1(JObject request) => "1";
            public static JToken Adapter2(JObject request) => "2";
            public static JToken Adapter3(JObject request) => "3";
            public static JToken AdapterAppend(JObject request)
            {
                return $"{request["string1"]}{request["string2"]}";
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
                string tester = "Test5";
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
                            AdapterNodes = new CodeTemplate.AdapterNode[]
                            {
                                new CodeTemplate.AdapterNode("Adapter1", $"{host}/api/testers/adapters/{tester}/Adapter1")
                            },
                            ParameterNodes = new CodeTemplate.ParameterNode[]
                            {
                                new CodeTemplate.ParameterNode("Result").FromTemplate(new CodeTemplate.TemplateNode()
                                {
                                    Url = $"{host}/api/testers/templates/{tester}/Template1",
                                    AdapterNodes = new CodeTemplate.AdapterNode[]
                                    {
                                        new CodeTemplate.AdapterNode("Adapter2", $"{host}/api/testers/adapters/{tester}/Adapter2")
                                    },
                                    ParameterNodes = new CodeTemplate.ParameterNode[]
                                    {
                                        new CodeTemplate.ParameterNode("Result").FromTemplate(new CodeTemplate.TemplateNode()
                                        {
                                            Url = $"{host}/api/testers/templates/{tester}/Template1",
                                            AdapterNodes = new CodeTemplate.AdapterNode[]
                                            {
                                                new CodeTemplate.AdapterNode("Adapter3", $"{host}/api/testers/adapters/{tester}/Adapter3")
                                            },
                                            ParameterNodes = new CodeTemplate.ParameterNode[]
                                            {
                                                new CodeTemplate.ParameterNode("Result").FromTemplate(new CodeTemplate.TemplateNode()
                                                {
                                                    Url = $"{host}/api/testers/templates/{tester}/Template1",
                                                    AdapterNodes = new CodeTemplate.AdapterNode[]
                                                    {
                                                        new CodeTemplate.AdapterNode("Adapter4", $"{host}/api/testers/adapters/{tester}/AdapterAppend")
                                                        {
                                                            RequestNodes = new CodeTemplate.RequestNode[]
                                                            {
                                                                new CodeTemplate.RequestNode("string1").FromAdapter("Adapter1"),
                                                                new CodeTemplate.RequestNode("string2").FromAdapter("Adapter2")
                                                            }
                                                        },
                                                        new CodeTemplate.AdapterNode("Adapter5", $"{host}/api/testers/adapters/{tester}/AdapterAppend")
                                                        {
                                                            RequestNodes = new CodeTemplate.RequestNode[]
                                                            {
                                                                new CodeTemplate.RequestNode("string1").FromAdapter("Adapter4"),
                                                                new CodeTemplate.RequestNode("string2").FromAdapter("Adapter3")
                                                            }
                                                        }
                                                    },
                                                    ParameterNodes = new CodeTemplate.ParameterNode[]
                                                    {
                                                        new CodeTemplate.ParameterNode("Result").FromAdapter("Adapter5")
                                                    }
                                                })
                                            }
                                        })
                                    }
                                })
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

                return result[0] == "123";
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
