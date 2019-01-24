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
            public string Name { get; set; }
            public string Description { get; set; }
            public string Context { get; set; }
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
                    new SqlParameter("@Name", request.Name),
                    new SqlParameter("@Description", request.Description),
                    new SqlParameter("@Context", request.Context.Replace("\r\n", "\n").Replace("\n", "\r\n")),
                    new SqlParameter("@Author", 0 as object),
                    new SqlParameter("@Owner", 0 as object)));
                return id;
            }
        }
    }
}
