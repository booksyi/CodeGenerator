using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.RequestNodes.Handlers
{
    public class UpdateRequestNodeById
    {
        public class Request : IRequest<uint>
        {
            public int Id { get; set; }
            public RequestNode Node { get; set; }
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
                await sqlHelper.ExecuteScalarAsync(@"
                    UPDATE CodeGeneratorRequestNode 
                    SET    [Node] = @Node 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id),
                    new SqlParameter("@Node", JsonConvert.SerializeObject(request.Node)));
                return 0;
            }
        }
    }
}
