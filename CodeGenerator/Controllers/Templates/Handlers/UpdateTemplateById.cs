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
    public class UpdateTemplateById
    {
        public class Request : IRequest<uint>
        {
            public DbTemplate Template { get; set; }
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
                    SET    [Name] = @Name, 
                           [Description] = @Description, 
                           Context = @Context, 
                           UpdateDate = @UpdateDate 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Template.Id),
                    new SqlParameter("@Name", request.Template.Name),
                    new SqlParameter("@Description", request.Template.Description),
                    new SqlParameter("@Context", request.Template.Context),
                    new SqlParameter("@UpdateDate", request.Template.UpdateDate));
                return 0;
            }
        }
    }
}
