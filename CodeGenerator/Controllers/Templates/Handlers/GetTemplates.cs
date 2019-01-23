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
    public class GetTemplates
    {
        public class Request : IRequest<IEnumerable<DbTemplate>>
        {
        }

        public class Handler : IRequestHandler<Request, IEnumerable<DbTemplate>>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<IEnumerable<DbTemplate>> Handle(Request request, CancellationToken token)
            {
                var templates = (await sqlHelper.ExecuteDataTableAsync(@"
                    SELECT * 
                    FROM   CodeGeneratorTemplate ")).Rows.ToModels<DbTemplate>();
                return templates;
            }
        }
    }
}
