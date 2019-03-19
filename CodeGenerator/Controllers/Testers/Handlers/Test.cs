using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            public string Tester { get; set; }
        }

        public class TestCase
        {
            public string Tester { get; set; }
            public bool Pass { get; set; } = true;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public Exception Exception { get; set; } = null;
        }

        public class Response
        {
            public IEnumerable<TestCase> Result { get; set; }
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
                    if (string.IsNullOrWhiteSpace(request.Tester) || request.Tester == tester.Name)
                    {
                        Type testerType = assembly.GetType(tester.FullName);
                        Type testerRequestType = assembly.GetType($"{tester.FullName}+Request");
                        TestCase testCase = new TestCase() { Tester = testerType.Name };
                        DescriptionAttribute descAttr = testerType.GetCustomAttribute<DescriptionAttribute>();
                        if (descAttr != null)
                        {
                            testCase.Tester = $"{testCase.Tester} ({descAttr.Description})";
                        }
                        try
                        {
                            IRequest<uint> testerRequest = Activator.CreateInstance(testerRequestType) as IRequest<uint>;
                            await mediator.Send(testerRequest);
                            testCase.Pass = true;
                        }
                        catch (Exception exception)
                        {
                            testCase.Pass = false;
                            testCase.Exception = exception;
                        }
                        cases.Add(testCase);
                    }
                }

                return new Response() { Result = cases };
            }
        }
    }
}
