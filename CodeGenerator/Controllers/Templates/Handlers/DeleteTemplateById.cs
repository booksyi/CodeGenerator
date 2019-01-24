using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Templates.Handlers
{
    public class DeleteTemplateById
    {
        public class Request : IRequest<uint>
        {
            internal int Id { get; set; }
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
                    DELETE FROM CodeGeneratorTemplate 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id));
                return 0;
            }
        }
    }
}
