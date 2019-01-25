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
        public class Request : IRequest<Test.TestCase>
        {
        }

        // $"{host}/api/testers/templates/{Tester}/{Template}"
        public class Templates
        {
            public const string Template1 = @"";
        }

        // $"{host}/api/testers/adapters/{Tester}/{Adapter}
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
                    { "Name", "ABC" }
                };

                // do somethings test

                return true;
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
