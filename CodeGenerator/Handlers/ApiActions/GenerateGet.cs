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
    public class GenerateGet
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
                GenerateNode node = new GenerateNode() { ApplyFilePath = @"Templates\CSharp\ApiActions\Get.html" };
                node.AppendChild("ProjectName", request.ProjectName);
                node.AppendChild("ModelName", request.TableSchema.ForCs.ModelName);
                node.AppendChild("PluralModelName", pluralizer.Pluralize(request.TableSchema.ForCs.ModelName));
                node.AppendChild("PluralModelObjectName", pluralizer.Pluralize(request.TableSchema.ForCs.ModelName).LowerFirst());

                node.AppendChild("Properties", request.TableSchema.Fields
                    .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));

                node.AppendChild(await mediator.Send(
                    new GenerateConstructor.Request()
                    {
                        TypeName = "Row",
                        Parameters = new string[] { $"{request.TableSchema.ForCs.ModelName} {request.TableSchema.ForCs.ModelName.LowerFirst()}" },
                        InnerCodes = request.TableSchema.Fields
                            .Select(x => $"this.{x.Name} = {request.TableSchema.ForCs.ModelName.LowerFirst()}.{x.Name};")
                    })).ChangeKey("Constructor");

                node.AppendChild(await mediator.Send(
                    new GenerateClassDependencyInjection.Request()
                    {
                        ClassName = "Handler",
                        Fields = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("IMediator", "mediator"),
                            new KeyValuePair<string, string>("DatabaseContext", "context")
                        }
                    })).ChangeKey("DependencyInjection");

                return node;
            }
        }
    }
}
