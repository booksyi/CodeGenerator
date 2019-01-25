using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Testers.Handlers
{
    public class Test
    {
        public class Request : IRequest<Response>
        {
        }

        public class TestCase
        {
            public string Tester { get; set; }
            public bool Pass { get; set; } = false;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Exception Exception { get; set; } = null;
        }

        public class Response : MultiResponse<TestCase>
        {
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IMediator mediator;
            private readonly SqlHelper sqlHelper;
            public Handler(IMediator mediator, SqlHelper sqlHelper)
            {
                this.mediator = mediator;
                this.sqlHelper = sqlHelper;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                List<TestCase> cases = new List<TestCase>();

                Assembly assembly = Assembly.GetEntryAssembly();
                Type[] testers = assembly
                    .GetTypes()
                    .Where(x =>
                        x.Namespace == "CodeGenerator.Controllers.Testers.Handlers.TestCases"
                        && x.ReflectedType == null)
                    .ToArray();

                foreach (Type tester in testers)
                {
                    Type testerRequestType = assembly.GetType($"{tester.FullName}+Request");
                    IRequest<TestCase> testerRequest = Activator.CreateInstance(testerRequestType) as IRequest<TestCase>;
                    cases.Add(await mediator.Send(testerRequest));
                }

                return new Response() { Result = cases };
            }
        }
    }
}
