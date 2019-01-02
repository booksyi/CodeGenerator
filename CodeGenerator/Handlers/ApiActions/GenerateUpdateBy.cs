using CodeGenerator.Data;
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
    public class GenerateUpdateBy
    {
        public class Request : IRequest<GenerateNode>
        {
            public DbTableSchema TableSchema { get; set; }

            public string ProjectName { get; set; }
            public FindBy By { get; set; }
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
                string template = await File.ReadAllTextAsync(@"Templates\CSharp\ApiActions\UpdateBy.html");
                GenerateNode node = new GenerateNode(template);
                node.AppendChild("ProjectName", request.ProjectName);
                node.AppendChild("ModelName", request.TableSchema.ForCs.ModelName);
                node.AppendChild("ModelObjectName", request.TableSchema.ForCs.ModelName.LowerFirst());
                node.AppendChild("PluralModelName", pluralizer.Pluralize(request.TableSchema.ForCs.ModelName));
                node.AppendChild("By", request.By.ToString());

                if (request.By == FindBy.Id)
                {
                    node.AppendChild("RequestProperties", request.TableSchema.Fields
                        .Where(x => x.IsIdentity || (x.IsPrimaryKey == false))
                        .Select(x => $"{(x.IsIdentity ? "internal" : "public")} {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));
                }
                else if (request.By == FindBy.Key)
                {
                    node.AppendChild("RequestProperties", request.TableSchema.Fields
                        .Where(x => x.IsPrimaryKey || (x.IsIdentity == false))
                        .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));
                }

                node.AppendChild("ResponseProperties", request.TableSchema.Fields
                    .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));

                node.AppendChild(await mediator.Send(
                    new GenerateConstructor.Request()
                    {
                        TypeName = "Response",
                        Parameters = new string[] { $"{request.TableSchema.ForCs.ModelName} {request.TableSchema.ForCs.ModelName.LowerFirst()}" },
                        InnerCodes = request.TableSchema.Fields
                            .Select(x => $"this.{x.Name} = {request.TableSchema.ForCs.ModelName.LowerFirst()}.{x.Name};")
                    })).Rename("ResponseConstructor");

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

                if (request.By == FindBy.Id)
                {
                    node.AppendChild("FindFilter", request.TableSchema.Fields
                        .Where(x => x.IsIdentity)
                        .Select(x => $"x.{x.Name} == request.{x.Name}"));
                }
                else if (request.By == FindBy.Key)
                {
                    node.AppendChild("FindFilter", request.TableSchema.Fields
                        .Where(x => x.IsPrimaryKey)
                        .Select(x => $"x.{x.Name} == request.{x.Name}"));
                }

                node.AppendChild("SetValues", request.TableSchema.Fields
                    .Where(x => x.IsIdentity == false)
                    .Where(x => x.IsPrimaryKey == false)
                    .Select(x => $"{request.TableSchema.ForCs.ModelName.LowerFirst()}.{x.Name} = request.{x.Name};"));

                return node;
            }
        }
    }
}
