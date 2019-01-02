using HelpersForCore;
using MediatR;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Handlers
{
    public class GenerateApiController
    {
        public class Request : IRequest<GenerateNode>
        {
            public DbTableSchema TableSchema { get; set; }
            public string ProjectName { get; set; }
        }

        public class Handler : IRequestHandler<Request, GenerateNode>
        {
            private readonly IMediator mediator;
            private readonly Pluralizer pluralizer;

            public Handler(IMediator mediator, Pluralizer pluralizer)
            {
                this.mediator = mediator;
                this.pluralizer = pluralizer;
            }

            public async Task<GenerateNode> Handle(Request request, CancellationToken token)
            {
                bool useIdentityAsPrimaryKey =
                    request.TableSchema.PrimaryKeys.Count() == 1
                    && request.TableSchema.PrimaryKeys.First().Name == request.TableSchema.Identity.Name;
                string template = useIdentityAsPrimaryKey ?
                    await File.ReadAllTextAsync(@"Templates\CSharp\ApiControllerWithoutByKey.html") :
                    await File.ReadAllTextAsync(@"Templates\CSharp\ApiController.html");
                GenerateNode node = new GenerateNode(template);
                node.AppendChild("ProjectName", request.ProjectName);
                node.AppendChild("ModelName", request.TableSchema.ForCs.ModelName);
                node.AppendChild("PluralModelName", pluralizer.Pluralize(request.TableSchema.ForCs.ModelName));
                node.AppendChild("IdentityFieldName", request.TableSchema.Identity.Name);
                node.AppendChild(await mediator.Send(
                    new GenerateClassDependencyInjection.Request()
                    {
                        ClassName = $"{pluralizer.Pluralize(request.TableSchema.ForCs.ModelName)}Controller",
                        Fields = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("IMediator", "mediator")
                        }
                    })).Rename("DependencyInjection");
                return node;
            }
        }
    }
}
