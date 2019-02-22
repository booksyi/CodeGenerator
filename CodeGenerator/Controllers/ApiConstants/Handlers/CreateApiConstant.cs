using CodeGenerator.Data.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.ApiConstants.Handlers
{
    public class CreateApiConstant
    {
        public class Request : IRequest<DbApiConstant>
        {
            public string Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbApiConstant>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<DbApiConstant> Handle(Request request, CancellationToken token)
            {
                DbApiConstant apiConstant = new DbApiConstant()
                {
                    Result = request.Result
                };
                await context.ApiConstants.AddAsync(apiConstant);
                await context.SaveChangesAsync();
                return apiConstant;
            }
        }
    }
}
