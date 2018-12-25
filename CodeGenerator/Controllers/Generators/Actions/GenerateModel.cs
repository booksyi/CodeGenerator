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
    public class GenerateModel
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

                string templateModel = await File.ReadAllTextAsync(@"Templates\Model\Model.txt");
                string templateModelProperty = await File.ReadAllTextAsync(@"Templates\Model\ModelProperty.txt");
                string templateModelPropertySummary = await File.ReadAllTextAsync(@"Templates\Model\ModelPropertySummary.txt");

                string code = CSharpHelper.Generate(
                    templateModel,
                    new Dictionary<string, string>()
                    {
                        { "ProjectName", request.ProjectName },
                        { "TableName", schema.TableName },
                        { "Singular", singular.UpperFirst() },
                        { "Properties", string.Join("\r\n\r\n", schema.Fields.Select(x => CSharpHelper.Generate(
                            templateModelProperty, new Dictionary<string, string>()
                            {
                                { "Summary", string.IsNullOrWhiteSpace(x.Description) == false ? CSharpHelper.Generate(templateModelPropertySummary, new Dictionary<string, string>() { { "Text", x.Description } }) : null },
                                { "Attributes", x.CsAttributes != null && x.CsAttributes.Any() ?  string.Join("\r\n", x.CsAttributes) : null},
                                { "Class", x.CsType },
                                { "PropertyName", x.Name },
                            }))) },
                    });
                await File.WriteAllTextAsync(Path.Combine(request.OutputPath, $"{singular.UpperFirst()}.cs"), code);
                return new ResponseBase<bool>(true);
            }
        }
    }
}
