using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.TempApi.Handlers
{
    public class GetTempApiById
    {
        public class Request : IRequest<DbTempApi>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbTempApi>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<DbTempApi> Handle(Request request, CancellationToken token)
            {
                DbTempApi tempApi = (await sqlHelper.ExecuteFirstRowAsync(@"
                    SELECT * 
                    FROM   CodeGeneratorTempApi 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id))).ToModel<DbTempApi>();
                return tempApi;
            }
        }
    }
}
