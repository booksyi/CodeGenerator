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
    public class GenerateConstructor
    {
        public class Request : IRequest<GenerateNode>
        {
            public string TypeName { get; set; }
            public IEnumerable<string> Parameters { get; set; }
            public IEnumerable<string> InnerCodes { get; set; }
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
                string template = await File.ReadAllTextAsync(@"Templates\CSharp\Constructor.html");
                GenerateNode node = new GenerateNode(template);
                node.AppendChild("TypeName", request.TypeName);
                node.AppendChild("Parameters", request.Parameters);
                node.AppendChild("InnerCodes", request.InnerCodes);
                return node;
            }
        }
    }
}
