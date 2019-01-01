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
    public class GenerateMethodParameter
    {
        public class Request : IRequest<GenerateNode>
        {
            public string Prefix { get; set; }
            public string TypeName { get; set; }
            public string ObjectName { get; set; }
        }

        public class Handler : IRequestHandler<Request, GenerateNode>
        {
            public Handler()
            {
            }

            public async Task<GenerateNode> Handle(Request request, CancellationToken token)
            {
                string template = string.IsNullOrWhiteSpace(request.Prefix) ?
                    await File.ReadAllTextAsync(@"Templates\CSharp\MethodParameterWithoutPrefix.html") :
                    await File.ReadAllTextAsync(@"Templates\CSharp\MethodParameter.html");
                GenerateNode node = new GenerateNode(template);
                node.AppendChild("Prefix", request.Prefix);
                node.AppendChild("TypeName", request.TypeName);
                node.AppendChild("ObjectName", request.ObjectName);
                return node;
            }
        }
    }
}
