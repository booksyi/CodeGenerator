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
    public class CreateTempApi
    {
        public class Request : IRequest<int>
        {
            public string Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, int>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<int> Handle(Request request, CancellationToken token)
            {
                int id = Convert.ToInt32(await sqlHelper.ExecuteScalarAsync(@"
                    INSERT INTO CodeGeneratorTempApi 
                                ([Result]) 
                    output      inserted.Id 
                    VALUES      (@Result) ",
                    new SqlParameter("@Result", request.Result)));
                return id;
            }
        }
    }
}
