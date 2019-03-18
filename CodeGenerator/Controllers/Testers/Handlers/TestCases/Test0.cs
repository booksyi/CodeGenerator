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
    public class Test0
    {
        public class Request : IRequest<uint>
        {
        }

        // $"{host}/api/testers/templates/{Tester}/{Template}"
        public class Templates
        {
            public static string Template1(JObject request)
            {
                return @"";
            }
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
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
                return true;
                string host = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";
                JObject httpRequest = new JObject()
                {
                    { "Name", "ABC" }
                };

                CodeTemplate template = new CodeTemplate()
                {
                    Inputs = new CodeTemplate.Input[]
                    {
                        new CodeTemplate.Input()
                    },
                    TemplateNodes = new CodeTemplate.TemplateNode[]
                    {
                        new CodeTemplate.TemplateNode()
                        {
                        }
                    }
                };

                List<string> result = new List<string>();
                var generateNodes = await template.ToGenerateNodesAsync(httpRequest);
                foreach (var generateNode in generateNodes)
                {
                    result.Add(await generateNode.GenerateAsync());
                }

                return result[0] == "";
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
