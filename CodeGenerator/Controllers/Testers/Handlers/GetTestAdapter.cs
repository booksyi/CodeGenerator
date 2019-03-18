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
            internal JToken InputData { get; set; }
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
                var value = method.Invoke(null, new object[] { request.InputData });
                return JToken.FromObject(value);
            }
        }
    }
}
