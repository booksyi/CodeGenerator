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
    public class CreateRequestNode
    {
        public class Request : IRequest<int>
        {
            public RequestNode Node { get; set; }
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
                    INSERT INTO CodeGeneratorRequestNode 
                    output      inserted.Id 
                    VALUES      (@Node) ",
                    new SqlParameter("@Node", JsonConvert.SerializeObject(
                        request.Node,
                        Formatting.None,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }))));
                return id;
            }
        }
    }
}
