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
    public class GetRequestNodeById
    {
        public class Request : IRequest<RequestNode>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, RequestNode>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<RequestNode> Handle(Request request, CancellationToken token)
            {
                string json = Convert.ToString(await sqlHelper.ExecuteScalarAsync(@"
                    SELECT [Node] 
                    FROM   CodeGeneratorRequestNode 
                    WHERE  Id = @Id ",
                    new SqlParameter("@Id", request.Id)));
                RequestNode node = JsonConvert.DeserializeObject<RequestNode>(json);
                return node;
            }
        }
    }
}
