using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.RequestNodes.Handlers
{
    public class ToGenerateNodes
    {
        public class Request : IRequest<IEnumerable<GenerateNode>>
        {
            internal int Id { get; set; }
            public JObject Body { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<GenerateNode>>
        {
            private readonly IMediator mediator;
            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<IEnumerable<GenerateNode>> Handle(Request request, CancellationToken token)
            {
                RequestNode node = await mediator.Send(new GetRequestNodeById.Request()
                {
                    Id = request.Id
                });
                //RequestNode[] nodes = (await node.BuildComplex()).ToArray();
                return await node.ToGenerateNode(request.Body);
                //return (await Task.WhenAll(nodes.Select(async x => await x.ToGenerateNode(request.Body)))).SelectMany(x => x);
            }
        }
    }
}
