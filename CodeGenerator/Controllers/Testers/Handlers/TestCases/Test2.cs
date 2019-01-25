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
            public const string Template1 = @"{{# Result }}";
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
        public class Adapters
        {
            public static JToken Adapter1(JObject query, JObject body)
            {
                return JToken.FromObject(new { Name1 = $"{query["Name"]}1" });
            }
            public static JToken Adapter2(JObject query, JObject body)
            {
                return JToken.FromObject(new { Name2 = $"{query["Name"]}2" });
            }
            public static JToken Adapter3(JObject query, JObject body)
            {
                return JToken.FromObject(new { Name3 = $"{body["Name"]}3" });
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
                string host = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
                JObject httpRequest = new JObject()
                {
                    { "Name", "ABC" }
                };

                RequestNode node = new RequestNode()
                {
                    From = RequestFrom.Template,
                    TemplateUrl = $"{host}/api/testers/templates/Test2/Template1",
                    AdapterNodes = new Dictionary<string, AdapterNode>()
                    {
                        {
                            "Adapter1", new AdapterNode()
                            {
                                HttpMethod = AdapterHttpMethod.Get,
                                Url = $"{host}/api/testers/adapters/Test2/Adapter1",
                                RequestNodes = new Dictionary<string, RequestSimpleNode>()
                                {
                                    {
                                        "Name", new RequestSimpleNode()
                                        {
                                            From = RequestSimpleFrom.HttpRequest,
                                            HttpRequestKey = "Name"
                                        }
                                    }
                                }
                            }
                        },
                        {
                            "Adapter2", new AdapterNode()
                            {
                                HttpMethod = AdapterHttpMethod.Get,
                                Url = $"{host}/api/testers/adapters/Test2/Adapter2",
                                RequestNodes = new Dictionary<string, RequestSimpleNode>()
                                {
                                    {
                                        "Name", new RequestSimpleNode()
                                        {
                                            From = RequestSimpleFrom.Adapter,
                                            AdapterName = "Adapter1",
                                            AdapterPropertyName = "Name1"
                                        }
                                    }
                                }
                            }
                        },
                        {
                            "Adapter3", new AdapterNode()
                            {
                                HttpMethod = AdapterHttpMethod.Post,
                                Url = $"{host}/api/testers/adapters/Test2/Adapter3",
                                RequestNodes = new Dictionary<string, RequestSimpleNode>()
                                {
                                    {
                                        "Name", new RequestSimpleNode()
                                        {
                                            From = RequestSimpleFrom.Adapter,
                                            AdapterName = "Adapter2",
                                            AdapterPropertyName = "Name2"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                    {
                        {
                            "Result", new RequestNode()
                            {
                                From = RequestFrom.Adapter,
                                AdapterName = "Adapter3",
                                AdapterPropertyName = "Name3"
                            }
                        }
                    }
                };

                List<string> result = new List<string>();
                var generateNodes = await node.ToGenerateNodeAsync(httpRequest);
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
