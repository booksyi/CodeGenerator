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
        public class Request : IRequest<IEnumerable<GeneratorsResource>>
        {
            internal int Id { get; set; }
            public JObject Body { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<GeneratorsResource>>
        {
            private readonly IMediator mediator;
            private readonly SqlHelper sqlHelper;
            public Handler(IMediator mediator, SqlHelper sqlHelper)
            {
                this.mediator = mediator;
                this.sqlHelper = sqlHelper;
            }

            public async Task<IEnumerable<GeneratorsResource>> Handle(Request request, CancellationToken token)
            {
                var nodes = await mediator.Send(new ToGenerateNodes.Request()
                {
                    Id = request.Id,
                    Body = request.Body
                });

                List<GeneratorsResource> resources = new List<GeneratorsResource>();
                foreach (var node in nodes)
                {
                    string text = await node.GenerateAsync();
                    resources.Add(new GeneratorsResource("", text));
                }

                return resources;
            }
        }
    }
}
