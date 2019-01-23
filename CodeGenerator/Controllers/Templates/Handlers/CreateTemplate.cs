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
    public class CreateTemplate
    {
        public class Request : IRequest<int>
        {
            public DbTemplate Template { get; set; }
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
                    INSERT INTO CodeGeneratorTemplate 
                                ([Name], 
                                 [Description], 
                                 Context, 
                                 Author, 
                                 [Owner]) 
                    output      inserted.Id 
                    VALUES      (@Name, 
                                 @Description, 
                                 @Context, 
                                 @Author, 
                                 @Owner) ",
                    new SqlParameter("@Name", request.Template.Name),
                    new SqlParameter("@Description", request.Template.Description),
                    new SqlParameter("@Context", request.Template.Context),
                    new SqlParameter("@Author", request.Template.Author),
                    new SqlParameter("@Owner", request.Template.Owner)));
                return id;
            }
        }
    }
}
