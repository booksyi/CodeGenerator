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
        public class Request : IRequest<DbSchema.Table>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbSchema.Table>
        {
            public Handler()
            {
            }

            public async Task<DbSchema.Table> Handle(Request request, CancellationToken token)
            {
                DbSchema.Table tableSchema =
                    CodingHelper.GetDbTableSchema(
                        request.ConnectionString,
                        request.TableName);
                return tableSchema;
            }
        }
    }
}
