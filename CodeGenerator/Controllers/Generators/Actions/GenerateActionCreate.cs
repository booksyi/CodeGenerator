using HelpersForCore;
using MediatR;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Actions
{
    public class GenerateActionCreate
    {
        public class Request : IRequest<ResponseBase<bool>>
        {
            public string ProjectName { get; set; }
            public string TableName { get; set; }
            public string ModelName { get; set; }
            public string OutputPath { get; set; }
        }

        public class Handler : IRequestHandler<Request, ResponseBase<bool>>
        {
            private readonly Pluralizer pluralizer;
            public Handler(Pluralizer pluralizer)
            {
                this.pluralizer = pluralizer;
            }

            public async Task<ResponseBase<bool>> Handle(Request request, CancellationToken token)
            {
                var schema = CodingHelper.GetDbTableSchema("Server=LAPTOP-RD9P71LP\\SQLEXPRESS;Database=TEST1;UID=sa;PWD=1234;", request.TableName);
                string singular = pluralizer.Singularize(request.ModelName).LowerFirst();
                string plural = pluralizer.Pluralize(request.ModelName).LowerFirst();

                string template = await File.ReadAllTextAsync(@"Templates\Api\Mediator\Actions\Create.txt");
                string code = CSharpHelper.Generate(template,
                    new Dictionary<string, string>()
                    {
                        { "ProjectName", request.ProjectName },
                        { "Singular", singular.UpperFirst() },
                        { "singular", singular },
                        { "Plural", plural.UpperFirst() },
                        { "plural", plural },
                    });
                await File.WriteAllTextAsync(Path.Combine(request.OutputPath, $"Create{singular.UpperFirst()}.cs"), code);
                return new ResponseBase<bool>(true);
            }
        }
    }
}
