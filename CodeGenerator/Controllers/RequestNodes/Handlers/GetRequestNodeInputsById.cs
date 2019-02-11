using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.RequestNodes.Handlers
{
    public class GetRequestNodeInputsById
    {
        public class Request : IRequest<IEnumerable<Input>>
        {
            internal int Id { get; set; }
        }

        public class Input
        {
            public string Key { get; set; }
            public string[] Descriptions { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
            public string Regex { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<Input>>
        {
            private readonly IMediator mediator;
            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<IEnumerable<Input>> Handle(Request request, CancellationToken token)
            {
                List<Input> inputs = new List<Input>();
                RequestNode node = await mediator.Send(new GetRequestNodeById.Request()
                {
                    Id = request.Id
                });
                inputs.AddRange(node.GetAllRequestKeys()
                    .Select(x =>
                        new Input()
                        {
                            Key = x.Key,
                            Descriptions = x.Value,
                            Type = "Text"
                        }));
                return inputs;
            }
        }
    }
}
