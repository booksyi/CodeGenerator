using HelpersForCore;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Handlers
{
    public class Generate
    {
        public class Request : IRequest<Response[]>
        {
            internal int Id { get; set; }
            public JObject Body { get; set; }
        }

        public class Response
        {
            public string Name { get; set; }
            public string Text { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response[]>
        {
            private readonly IMediator mediator;
            private readonly SqlHelper sqlHelper;
            public Handler(IMediator mediator, SqlHelper sqlHelper)
            {
                this.mediator = mediator;
                this.sqlHelper = sqlHelper;
            }

            public async Task<Response[]> Handle(Request request, CancellationToken token)
            {
                var nodes = await mediator.Send(new ToGenerateNodes.Request()
                {
                    Id = request.Id,
                    Body = request.Body
                });

                List<Response> resources = new List<Response>();
                foreach (var node in nodes)
                {
                    resources.Add(new Response()
                    {
                        Name = node.ApplyKey,
                        Text = await node.GenerateAsync()
                    });
                }
                return resources.ToArray();
            }
        }
    }
}
