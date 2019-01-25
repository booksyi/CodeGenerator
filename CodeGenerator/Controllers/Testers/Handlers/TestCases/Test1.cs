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
    /// 測試樣板參數 陣列 與 分隔符號
    /// </summary>
    public class Test1
    {
        public class Request : IRequest<Test.TestCase>
        {
        }

        public class Templates
        {
            public const string Template1 = @"{{# Names, + }}";
        }

        public class Adapters
        {
            public static JToken Adapter1(JObject query, JObject body)
            {
                return JToken.FromObject(new string[] { "AA", "BB", "CC" });
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
                };

                RequestNode node = new RequestNode()
                {
                    From = RequestFrom.Template,
                    TemplateUrl = $"{host}/api/testers/templates/Test1/Template1",
                    AdapterNodes = new Dictionary<string, AdapterNode>()
                    {
                        {
                            "Adapter1", new AdapterNode()
                            {
                                HttpMethod = AdapterHttpMethod.Get,
                                Url = $"{host}/api/testers/adapters/Test1/Adapter1"
                            }
                        }
                    },
                    SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                    {
                        {
                            "Names", new RequestNode()
                            {
                                From = RequestFrom.Adapter,
                                AdapterName = "Adapter1"
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

                return result[0] == "AA+BB+CC";
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
