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
        public class Request : IRequest<Response>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
            public string FieldName { get; set; }
        }

        public class Response
        {
            public string Description { get; set; }
            public string[] Attributes { get; set; }
            public string TypeName { get; set; }
            public string PropertyName { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(request.ConnectionString, request.TableName);
                DbTableSchema.Field field = tableSchema.Fields.FirstOrDefault(x => x.Name == request.FieldName);
                var attributes = field.ForCs.EFAttributes.ToArray();
                return new Response()
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
