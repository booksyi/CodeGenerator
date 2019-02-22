using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Testers.Handlers.TestCases
{
    /// <summary>
    /// 測試從 HttpRequest 取值及串流 Adapter
    /// </summary>
    public class Test2
    {
        public class Request : IRequest<Test.TestCase>
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
            public static JToken Adapter1(JObject request)
            {
                return JToken.FromObject(new { Name1 = $"{request["Name"]}1" });
            }
            public static JToken Adapter2(JObject request)
            {
                return JToken.FromObject(new { Name2 = $"{request["Name"]}2" });
            }
            public static JToken Adapter3(JObject request)
            {
                return JToken.FromObject(new { Name3 = $"{request["Name"]}3" });
            }
        }

        public class Handler : IRequestHandler<Request, Test.TestCase>
        {
            private readonly IHttpContextAccessor httpContextAccessor;
            public Handler(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Test(Request request)
            {
                string tester = "Test2";
                string host = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
                JObject httpRequest = new JObject()
                {
                    { "Name", "ABC" }
                };

                CodeTemplate template = new CodeTemplate()
                {
                    Inputs = new CodeTemplate.Input[]
                    {
                        new CodeTemplate.Input("Name")
                    },
                    TemplateNodes = new CodeTemplate.TemplateNode[]
                    {
                        new CodeTemplate.TemplateNode()
                        {
                            Url = $"{host}/api/testers/templates/{tester}/Template1",
                            AdapterNodes = new CodeTemplate.AdapterNode[]
                            {
                                new CodeTemplate.AdapterNode("Adapter1", $"{host}/api/testers/adapters/{tester}/Adapter1")
                                {
                                    RequestNodes = new CodeTemplate.RequestNode[]
                                    {
                                        new CodeTemplate.RequestNode("Name").FromInput("Name")
                                    }
                                },
                                new CodeTemplate.AdapterNode("Adapter2", $"{host}/api/testers/adapters/{tester}/Adapter2")
                                {
                                    RequestNodes = new CodeTemplate.RequestNode[]
                                    {
                                        new CodeTemplate.RequestNode("Name").FromAdapter("Adapter1", "Name1")
                                    }
                                },
                                new CodeTemplate.AdapterNode("Adapter3", $"{host}/api/testers/adapters/{tester}/Adapter3")
                                {
                                    RequestNodes = new CodeTemplate.RequestNode[]
                                    {
                                        new CodeTemplate.RequestNode("Name").FromAdapter("Adapter2", "Name2")
                                    }
                                }
                            },
                            ParameterNodes = new CodeTemplate.ParameterNode[]
                            {
                                new CodeTemplate.ParameterNode("Result").FromAdapter("Adapter3", "Name3")
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
                
                return result[0] == "ABC123";
            }

            public async Task<Test.TestCase> Handle(Request request, CancellationToken token)
            {
                Test.TestCase testCase = new Test.TestCase() { Tester = this.GetType().ReflectedType.Name };
                try
                {
                    if (await Test(request) == false)
                    {
                        throw new Exception("測試結果不如預期");
                    }
                    testCase.Pass = true;
                }
                catch (Exception ex)
                {
                    testCase.Pass = false;
                    testCase.Exception = ex;
                }
                return testCase;
            }
        }
    }
}
