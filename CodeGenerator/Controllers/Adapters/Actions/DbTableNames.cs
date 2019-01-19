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
        public class Request : IRequest<Response>
        {
            public string ConnectionString { get; set; }
        }

        public class Response
        {
            public string[] TableNames { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Handler()
            {
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                var tableNames = CodingHelper.GetDbTableNames(request.ConnectionString).ToArray();
                return new Response()
                {
                    TableNames = tableNames
                };
            }
        }
    }
}
