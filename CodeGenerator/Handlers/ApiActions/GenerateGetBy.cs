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
    public class GenerateGetBy
    {
        public class Request : IRequest<GenerateNode>
        {
            public DbTableSchema TableSchema { get; set; }

            public string ProjectName { get; set; }
            public string ModelName { get; set; }
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
                string template = await File.ReadAllTextAsync(@"Templates\CSharp\ApiActions\GetBy.html");
                GenerateNode node = new GenerateNode(template);
                node.AppendChild("ProjectName", request.ProjectName);
                node.AppendChild("ModelName", request.ModelName);
                node.AppendChild("ModelObjectName", request.ModelName.LowerFirst());
                node.AppendChild("PluralModelName", pluralizer.Pluralize(request.ModelName));
                node.AppendChild("By", request.By.ToString());

                if (request.By == FindBy.Id)
                {
                    node.AppendChild("RequestProperties", request.TableSchema.Fields
                        .Where(x => x.IsIdentity)
                        .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));

                    node.AppendChild(await mediator.Send(
                        new GenerateConstructor.Request()
                        {
                            TypeName = "Request",
                            Parameters = request.TableSchema.Fields
                                .Where(x => x.IsIdentity)
                                .Select(x => $"{x.TypeName} {x.Name.LowerFirst()}"),
                            InnerCodes = request.TableSchema.Fields
                                .Where(x => x.IsIdentity)
                                .Select(x => $"this.{x.Name} = {x.Name.LowerFirst()};")
                        })).Rename("RequestConstructor");
                }
                else if (request.By == FindBy.Key)
                {
                    node.AppendChild("RequestProperties", request.TableSchema.Fields
                        .Where(x => x.IsPrimaryKey)
                        .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));

                    node.AppendChild(await mediator.Send(
                        new GenerateConstructor.Request()
                        {
                            TypeName = "Request",
                            Parameters = request.TableSchema.Fields
                                .Where(x => x.IsPrimaryKey)
                                .Select(x => $"{x.TypeName} {x.Name.LowerFirst()}"),
                            InnerCodes = request.TableSchema.Fields
                                .Where(x => x.IsPrimaryKey)
                                .Select(x => $"this.{x.Name} = {x.Name.LowerFirst()};")
                        })).Rename("RequestConstructor");
                }

                node.AppendChild("ResponseProperties", request.TableSchema.Fields
                    .Where(x => x.IsIdentity == false)
                    .Select(x => $"public {x.ForCs.TypeName} {x.Name} {{ get; set; }}"));

                node.AppendChild(await mediator.Send(
                    new GenerateConstructor.Request()
                    {
                        TypeName = "Response",
                        Parameters = new string[] { $"{request.ModelName} {request.ModelName.LowerFirst()}" },
                        InnerCodes = request.TableSchema.Fields
                            .Where(x => x.IsIdentity == false)
                            .Select(x => $"this.{x.Name} = {request.ModelName.LowerFirst()}.{x.Name};")
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

                return node;
            }
        }
    }
}
