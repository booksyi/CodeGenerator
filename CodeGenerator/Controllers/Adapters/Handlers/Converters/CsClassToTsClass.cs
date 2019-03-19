using AutoMapper;
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
        public class Request : IRequest<TsSchema.Class>
        {
            public CsSchema.Class CsClass { get; set; }
        }

        public class Handler : IRequestHandler<Request, TsSchema.Class>
        {
            private readonly IMapper mapper;
            public Handler(IMapper mapper)
            {
                this.mapper = mapper;
            }

            public async Task<TsSchema.Class> Handle(Request request, CancellationToken token)
            {
                return mapper.Map<TsSchema.Class>(request.CsClass);
            }
        }
    }
}
