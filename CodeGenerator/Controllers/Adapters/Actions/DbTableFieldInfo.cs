using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Actions
{
    public class DbTableFieldInfo
    {
        public class Request : IRequest<IEnumerable<KeyValuePair<string, string>>>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
            public string FieldName { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<KeyValuePair<string, string>>>
        {
            public Handler()
            {
            }

            public async Task<IEnumerable<KeyValuePair<string, string>>> Handle(Request request, CancellationToken token)
            {
                List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(request.ConnectionString, request.TableName);
                DbTableSchema.Field field = tableSchema.Fields.FirstOrDefault(x => x.Name == request.FieldName);
                result.Add(new KeyValuePair<string, string>("Description", field.Description));
                foreach (string attribute in field.ForCs.EFAttributes)
                {
                    result.Add(new KeyValuePair<string, string>("Attributes", attribute));
                }
                result.Add(new KeyValuePair<string, string>("TypeName", field.ForCs.TypeName));
                result.Add(new KeyValuePair<string, string>("PropertyName", field.Name));
                return result;
            }
        }
    }
}
