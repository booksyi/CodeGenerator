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
    public class GenerateModelProperty
    {
        public class Request : IRequest<GenerateNode>
        {
            public DbTableSchema.Field Field { get; set; }
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
                string template = await File.ReadAllTextAsync(@"Templates\CSharp\DeclareProperty.html");
                GenerateNode node = new GenerateNode(template);
                if (string.IsNullOrWhiteSpace(request.Field.Description) == false)
                {
                    node.AppendChild(await mediator.Send(
                        new GenerateModelPropertySummary.Request()
                        {
                            Text = request.Field.Description
                        })).Rename("Summary");
                }
                node.AppendChild("Attributes", request.Field.ForCs.EFAttributes);
                node.AppendChild("TypeName", request.Field.ForCs.TypeName);
                node.AppendChild("PropertyName", request.Field.Name);
                return node;
            }
        }
    }
}
