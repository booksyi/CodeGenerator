using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Handlers.StringProcess
{
    public class StringFormat
    {
        public class Request : IRequest<string>
        {
            public string Format { get; set; }
            public string[] Parameters { get; set; }
        }

        public class Handler : IRequestHandler<Request, string>
        {
            public Handler()
            {
            }

            public async Task<string> Handle(Request request, CancellationToken token)
            {
                return string.Format(request.Format, request.Parameters);
            }
        }
    }
}
