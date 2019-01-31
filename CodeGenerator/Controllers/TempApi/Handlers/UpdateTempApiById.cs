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
    public class UpdateTempApiById
    {
        public class Request : IRequest<uint>
        {
            internal int Id { get; set; }
            public string Result { get; set; }
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
                    UPDATE CodeGeneratorTempApi 
                    SET    Result = @Result 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id),
                    new SqlParameter("@Result", request.Result));
                return 0;
            }
        }
    }
}
