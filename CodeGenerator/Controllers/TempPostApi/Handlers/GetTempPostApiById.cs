using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.TempPostApi.Handlers
{
    public class GetTempPostApiById
    {
        public class Request : IRequest<DbTempPostApi>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbTempPostApi>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<DbTempPostApi> Handle(Request request, CancellationToken token)
            {
                DbTempPostApi tempPostApi = (await sqlHelper.ExecuteFirstRowAsync(@"
                    SELECT * 
                    FROM   CodeGeneratorTempPostApi 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id))).ToModel<DbTempPostApi>();
                return tempPostApi;
            }
        }
    }
}
