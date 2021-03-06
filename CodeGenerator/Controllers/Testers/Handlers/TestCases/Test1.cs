﻿using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Testers.Handlers.TestCases
{
    [Description("測試樣板參數 陣列 與 分隔符號")]
    public class Test1
    {
        public class Request : IRequest<uint>
        {
        }

        public class Templates
        {
            public static string Template1(JObject request)
            {
                return @"{{# Names, + }}";
            }
        }

        public class Adapters
        {
            public static JToken Adapter1(JObject request)
            {
                return JToken.FromObject(new string[] { "AA", "BB", "CC" });
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
                string tester = "Test1";
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
                                new CodeTemplate.ParameterNode("Names").FromAdapter("Adapter1")
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

                return result[0] == "AA+BB+CC";
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
