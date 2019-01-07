using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Handlers
{
    public class GenerateConstructorSetValue
    {
        public class Request : IRequest<GenerateNode>
        {
            public string Accept { get; set; }
            public string Deliver { get; set; }
        }

        public class Handler : IRequestHandler<Request, GenerateNode>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<GenerateNode> Handle(Request request, CancellationToken token)
            {
                GenerateNode node = new GenerateNode() { ApplyFilePath = @"Templates\CSharp\ConstructorSetValue.html" };
                node.AppendChild("Accept", request.Accept);
                node.AppendChild("Deliver", request.Deliver);
                return node;
            }
        }
    }
}
