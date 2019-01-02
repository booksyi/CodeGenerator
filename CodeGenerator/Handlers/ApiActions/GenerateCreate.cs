using HelpersForCore;
using MediatR;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Handlers.ApiActions
{
    public class GenerateCreate
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
                string template = await File.ReadAllTextAsync(@"Templates\CSharp\ApiActions\Create.html");
                GenerateNode node = new GenerateNode(template);
                node.AppendChild("ProjectName", request.ProjectName);
                node.AppendChild("ModelName", request.TableSchema.ForCs.ModelName);
                node.AppendChild("ModelObjectName", request.TableSchema.ForCs.ModelName.LowerFirst());
                node.AppendChild("PluralModelName", pluralizer.Pluralize(request.TableSchema.ForCs.ModelName));

                node.AppendChild("Properties", request.TableSchema.Fields
                    .Where(x => x.IsIdentity == false)
                    .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));

                node.AppendChild(await mediator.Send(
                    new GenerateConstructor.Request()
                    {
                        TypeName = "Response",
                        Parameters = new string[] { $"{request.TableSchema.ForCs.ModelName} {request.TableSchema.ForCs.ModelName.LowerFirst()}" },
                        InnerCodes = request.TableSchema.Fields
                            .Where(x => x.IsIdentity == false)
                            .Select(x => $"this.{x.Name} = {request.TableSchema.ForCs.ModelName.LowerFirst()}.{x.Name};")
                    })).Rename("Constructor");

                node.AppendChild(await mediator.Send(
                    new GenerateClassDependencyInjection.Request()
                    {
                        ClassName = "Handler",
                        Fields = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("IMediator", "mediator"),
                            new KeyValuePair<string, string>("DatabaseContext", "context")
                        }
                    })).Rename("DependencyInjection");
                
                node.AppendChild("SetValues", request.TableSchema.Fields
                    .Where(x => x.IsIdentity == false)
                    .Select(x => $"{request.TableSchema.ForCs.ModelName.LowerFirst()}.{x.Name} = request.{x.Name};"));
                return node;
            }
        }
    }
}
