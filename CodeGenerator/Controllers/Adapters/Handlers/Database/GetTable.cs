using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Handlers.Database
{
    public class GetTable
    {
        public class Request : IRequest<DbTableSchema>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbTableSchema>
        {
            public Handler()
            {
            }

            public async Task<DbTableSchema> Handle(Request request, CancellationToken token)
            {
                DbTableSchema tableSchema =
                    CodingHelper.GetDbTableSchema(
                        request.ConnectionString,
                        request.TableName);
                return tableSchema;
            }
        }
    }
}
