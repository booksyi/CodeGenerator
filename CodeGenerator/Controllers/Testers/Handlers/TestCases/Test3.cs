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
    public class Test3
    {
        public class Request : IRequest<Test.TestCase>
        {
        }

        // $"{host}/api/testers/templates/{Tester}/{Template}"
        public class Templates
        {
            public static string Template1(JObject request)
            {
                return @"<table>{{# Fields }}</table>";
            }

            public static string Template2(JObject request)
            {
                return @"<th>{{# FieldName }}</th>";
            }
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
        public class Adapters
        {
            public static JToken Adapter1(JObject request)
            {
                string tableName = Convert.ToString(request["TableName"]);
                if (tableName == "TableA")
                {
                    return JToken.FromObject(
                        new
                        {
                            TableName = "TableA",
                            Fields = new object[]
                            {
                                new { Name = "XX", DataType = "String" },
                                new { Name = "YY", DataType = "String" },
                                new { Name = "ZZ", DataType = "String" }
                            }
                        });
                }
                return JToken.FromObject(
                    new
                    {
                        TableName = "Unknow",
                        Fields = new object[]
                        {
                                new { Name = "AA", DataType = "String" },
                                new { Name = "BB", DataType = "String" },
                                new { Name = "CC", DataType = "String" }
                        }
                    });
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
                    { "Table", "TableA" }
                };

                RequestNode node = new RequestNode()
                {
                    From = RequestFrom.Template,
                    TemplateNode = new ApiNode()
                    {
                        Url = $"{host}/api/testers/templates/Test3/Template1"
                    },
                    SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                    {
                        {
                            "Fields", new RequestNode()
                            {
                                From = RequestFrom.Template,
                                TemplateNode = new ApiNode()
                                {
                                    Url = $"{host}/api/testers/templates/Test3/Template2"
                                },
                                AdapterNodes = new Dictionary<string, AdapterNode>()
                                {
                                    {
                                        "Adapter1", new AdapterNode()
                                        {
                                            HttpMethod = AdapterHttpMethod.Get,
                                            Url = $"{host}/api/testers/adapters/Test3/Adapter1",
                                            RequestNodes = new Dictionary<string, RequestSimpleNode>()
                                            {
                                                {
                                                    "TableName", new RequestSimpleNode()
                                                    {
                                                        From = RequestSimpleFrom.HttpRequest,
                                                        HttpRequestKey = "Table"
                                                    }
                                                }
                                            },
                                            ResponseConfine = "Fields",
                                            Type = AdapterType.Separation
                                        }
                                    }
                                },
                                SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                                {
                                    {
                                        "FieldName", new RequestNode()
                                        {
                                            From = RequestFrom.Adapter,
                                            AdapterName = "Adapter1",
                                            AdapterPropertyName = "Fields.Name"
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                List<string> result = new List<string>();
                var generateNodes = await node.ToGenerateNodesAsync(httpRequest);
                foreach (var generateNode in generateNodes)
                {
                    result.Add(await generateNode.GenerateAsync());
                }

                return result[0] == "<table><th>XX</th><th>YY</th><th>ZZ</th></table>";
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
