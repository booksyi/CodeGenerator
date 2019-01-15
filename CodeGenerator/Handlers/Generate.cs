using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Handlers
{
    public class Generate
    {
        public class Request : IRequest<GenerateNode>
        {
            public string Path { get; set; }
            public IEnumerable<KeyValuePair<string, string>> Parameters { get; set; }
        }

        public class Handler : IRequestHandler<Request, GenerateNode>
        {
            public Handler()
            {
            }

            public async Task<GenerateNode> Handle(Request request, CancellationToken token)
            {
                GenerateNode node = new GenerateNode() { ApplyFilePath = request.Path };
                foreach (var parameter in request.Parameters)
                {
                    node.AppendChild(parameter.Key, parameter.Value);
                }
                return node;
            }
        }
    }
}
