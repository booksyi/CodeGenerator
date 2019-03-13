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
        public class Request : IRequest<DbSchemaTable>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbSchemaTable>
        {
            public Handler()
            {
            }

            public async Task<DbSchemaTable> Handle(Request request, CancellationToken token)
            {
                DbSchemaTable tableSchema =
                    CodingHelper.GetDbTableSchema(
                        request.ConnectionString,
                        request.TableName);
                return tableSchema;
            }
        }
    }
}
