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
    public class GenerateModelPropertySummary
    {
        public class Request : IRequest<GenerateNode>
        {
            public string Text { get; set; }
        }

        public class Handler : IRequestHandler<Request, GenerateNode>
        {
            public Handler()
            {
            }

            public async Task<GenerateNode> Handle(Request request, CancellationToken token)
            {
                GenerateNode node = new GenerateNode() { ApplyFilePath = @"Templates\CSharp\Summary.html" };
                node.AppendChild("Text", request.Text);
                return node;
            }
        }
    }
}
