using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Testers.Handlers
{
    public class GetTestAdapter
    {
        public class Request : IRequest<JToken>
        {
            internal string Tester { get; set; }
            internal string Adapter { get; set; }
            internal JObject Query { get; set; }
            internal JObject Body { get; set; }
        }

        public class Handler : IRequestHandler<Request, JToken>
        {
            public Handler()
            {
            }

            public async Task<JToken> Handle(Request request, CancellationToken token)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                Type type = assembly.GetType(
                    $"CodeGenerator.Controllers.Testers.Handlers.TestCases.{request.Tester}+Adapters");
                var method = type.GetMethod(request.Adapter);
                var adapterValue = method.Invoke(null, new object[] { request.Query, request.Body });
                return JToken.FromObject(adapterValue);
            }
        }
    }
}
