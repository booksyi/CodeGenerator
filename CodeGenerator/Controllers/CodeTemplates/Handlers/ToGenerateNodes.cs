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

namespace CodeGenerator.Controllers.CodeTemplates.Handlers
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
                DbCodeTemplate codeTemplate = await mediator.Send(new GetCodeTemplateById.Request()
                {
                    Id = request.Id
                });
                CodeTemplate template = JsonConvert.DeserializeObject<CodeTemplate>(codeTemplate.Node);
                return await template.ToGenerateNodesAsync(request.Body);
            }
        }
    }
}
