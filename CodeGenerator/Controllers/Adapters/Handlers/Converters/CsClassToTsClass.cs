using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Handlers.Converters
{
    public class CsClassToTsClass
    {
        public class Request : IRequest<TsSchemaClass>
        {
            public CsSchemaClass CsClass { get; set; }
        }

        public class Handler : IRequestHandler<Request, TsSchemaClass>
        {
            public Handler()
            {
            }

            public async Task<TsSchemaClass> Handle(Request request, CancellationToken token)
            {
                return request.CsClass.ToTsClass();
            }
        }
    }
}
