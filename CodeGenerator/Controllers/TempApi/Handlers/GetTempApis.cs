using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.TempApi.Handlers
{
    public class GetTempApis
    {
        public class Request : IRequest<IEnumerable<DbTempApi>>
        {
        }

        public class Handler : IRequestHandler<Request, IEnumerable<DbTempApi>>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<IEnumerable<DbTempApi>> Handle(Request request, CancellationToken token)
            {
                var templates = (await sqlHelper.ExecuteDataTableAsync(@"
                    SELECT * 
                    FROM   CodeGeneratorTempApi ")).Rows.ToObjects<DbTempApi>();
                return templates;
            }
        }
    }
}
