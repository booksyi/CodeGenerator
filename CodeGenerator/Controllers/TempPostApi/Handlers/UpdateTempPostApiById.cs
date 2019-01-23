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
    public class UpdateTempPostApiById
    {
        public class Request : IRequest<uint>
        {
            public DbTempPostApi TempPostApi { get; set; }
        }

        public class Handler : IRequestHandler<Request, uint>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<uint> Handle(Request request, CancellationToken token)
            {
                await sqlHelper.ExecuteNonQueryAsync(@"
                    UPDATE CodeGeneratorTemplate 
                    SET    [Name] = @Result 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.TempPostApi.Id),
                    new SqlParameter("@Result", request.TempPostApi.Result));
                return 0;
            }
        }
    }
}
