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
    public class GenerateModel
    {
        public class Request : IRequest<GenerateNode>
        {
            public string ProjectName { get; set; }
            public DbTableSchema TableSchema { get; set; }
            public string ModelName { get; set; }
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
                GenerateNode node = new GenerateNode() { ApplyFilePath = @"Templates\CSharp\EntityFrameworkModel.html" };
                node.AppendChild("ProjectName", request.ProjectName);
                node.AppendChild("TableName", request.TableSchema.TableName);
                node.AppendChild("ModelName", request.ModelName);
                foreach (var field in request.TableSchema.Fields)
                {
                    node.AppendChild(await mediator.Send(
                        new GenerateModelProperty.Request()
                        {
                            Field = field
                        })).ChangeKey("Properties");
                }
                return node;
            }
        }
    }
}
