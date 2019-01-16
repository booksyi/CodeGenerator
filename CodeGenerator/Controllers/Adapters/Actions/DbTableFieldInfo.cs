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
        public class Request : IRequest<object>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
            public string FieldName { get; set; }
        }

        public class Handler : IRequestHandler<Request, object>
        {
            public Handler()
            {
            }

            public async Task<object> Handle(Request request, CancellationToken token)
            {
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(request.ConnectionString, request.TableName);
                DbTableSchema.Field field = tableSchema.Fields.FirstOrDefault(x => x.Name == request.FieldName);
                var attributes = field.ForCs.EFAttributes.ToArray();
                return new
                {
                    Description = field.Description,
                    Attributes = attributes,
                    TypeName = field.ForCs.TypeName,
                    PropertyName = field.Name
                };
            }
        }
    }
}
