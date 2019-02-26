using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Handlers
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
                Generator generator = await mediator.Send(new GetGeneratorById.Request()
                {
                    Id = request.Id
                });
                CodeTemplate codeTemplate = JsonConvert.DeserializeObject<CodeTemplate>(generator.Json);
                return await codeTemplate.ToGenerateNodesAsync(request.Body);
            }
        }
    }
}
