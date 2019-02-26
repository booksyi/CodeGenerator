using CodeGenerator.Data.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Constants.Handlers
{
    public class GetConstants
    {
        public class Request : IRequest<Constant[]>
        {
        }

        public class Handler : IRequestHandler<Request, Constant[]>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Constant[]> Handle(Request request, CancellationToken token)
            {
                Constant[] constants = context.Constants.ToArray();
                return constants;
            }
        }
    }
}
