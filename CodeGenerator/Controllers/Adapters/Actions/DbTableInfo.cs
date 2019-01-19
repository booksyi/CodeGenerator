using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Actions
{
    public class DbTableInfo
    {
        public class Request : IRequest<Response>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }
        public class Response
        {
            public string Identity { get; set; }
            public string[] PrimaryKeys { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(request.ConnectionString, request.TableName);
                var primaryKeys = tableSchema.PrimaryKeys.Select(x => x.Name).ToArray();
                return new Response()
                {
                    Identity = tableSchema.Identity.Name,
                    PrimaryKeys = primaryKeys
                };
            }
        }
    }
}
