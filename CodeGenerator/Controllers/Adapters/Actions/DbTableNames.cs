using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Actions
{
    public class DbTableNames
    {
        public class Request : IRequest<object>
        {
            public string ConnectionString { get; set; }
        }

        public class Handler : IRequestHandler<Request, object>
        {
            public Handler()
            {
            }

            public async Task<object> Handle(Request request, CancellationToken token)
            {
                var tableNames = CodingHelper.GetDbTableNames(request.ConnectionString).ToArray();
                return new
                {
                    TableNames = tableNames
                };
            }
        }
    }
}
