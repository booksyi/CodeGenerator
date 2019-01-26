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
    public class GetTestTemplate
    {
        public class Request : IRequest<string>
        {
            internal string Tester { get; set; }
            internal string Template { get; set; }
            internal JObject Query { get; set; }
        }

        public class Handler : IRequestHandler<Request, string>
        {
            public Handler()
            {
            }

            public async Task<string> Handle(Request request, CancellationToken token)
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                Type type = assembly.GetType(
                    $"CodeGenerator.Controllers.Testers.Handlers.TestCases.{request.Tester}+Templates");
                var method = type.GetMethod(request.Template);
                var value = method.Invoke(null, new object[] { request.Query });
                return value as string;
            }
        }
    }
}
