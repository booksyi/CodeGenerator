using CodeGenerator.Data.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.ApiConstants.Handlers
{
    public class GetApiConstants
    {
        public class Request : IRequest<DbApiConstant[]>
        {
        }

        public class Handler : IRequestHandler<Request, DbApiConstant[]>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<DbApiConstant[]> Handle(Request request, CancellationToken token)
            {
                DbApiConstant[] apiConstants = context.ApiConstants.ToArray();
                return apiConstants;
            }
        }
    }
}
