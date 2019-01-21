using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Actions
{
    public class DbTableFields
    {
        public class Request : IRequest<Response>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }

        public class Response
        {
            public DbTableSchema.Field[] Fields { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(request.ConnectionString, request.TableName);
                var fields = tableSchema.Fields.ToArray();
                return new Response()
                {
                    Fields = fields
                };
            }
        }
    }
}
