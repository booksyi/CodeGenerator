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
            internal int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Context { get; set; }
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
                    new SqlParameter("@Id", request.Id),
                    new SqlParameter("@Name", request.Name),
                    new SqlParameter("@Description", request.Description),
                    new SqlParameter("@Context", request.Context.Replace("\r\n", "\n").Replace("\n", "\r\n")),
                    new SqlParameter("@UpdateDate", DateTime.Now));
                return 0;
            }
        }
    }
}
