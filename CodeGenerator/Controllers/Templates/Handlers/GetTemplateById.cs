using CodeGenerator.Data.Models;
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
    public class GetTemplateById
    {
        public class Request : IRequest<DbTemplate>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbTemplate>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<DbTemplate> Handle(Request request, CancellationToken token)
            {
                DbTemplate template = (await sqlHelper.ExecuteFirstRowAsync(@"
                    SELECT * 
                    FROM   CodeGeneratorTemplate 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id))).ToObject<DbTemplate>();
                return template;
            }
        }
    }
}
