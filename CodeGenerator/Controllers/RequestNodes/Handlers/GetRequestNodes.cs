using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.RequestNodes.Handlers
{
    public class GetRequestNodes
    {
        public class Request : IRequest<IEnumerable<Response>>
        {
        }

        public class Response : DbRequestNode
        {
            public IEnumerable<string> Templates { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<Response>>
        {
            private readonly SqlHelper sqlHelper;
            public Handler(SqlHelper sqlHelper)
            {
                this.sqlHelper = sqlHelper;
            }

            public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken token)
            {
                var nodes = (await sqlHelper.ExecuteDataTableAsync(@"
                    SELECT * 
                    FROM   CodeGeneratorRequestNode ")).Rows.ToObjects<Response>();
                foreach (var node in nodes)
                {
                    CodeTemplate.TransactionParameterNode requestNode = JsonConvert.DeserializeObject<CodeTemplate.TransactionParameterNode>(node.Node);
                    node.Templates = requestNode.GetAllTemplates();
                }
                return nodes;
            }
        }
    }
}
