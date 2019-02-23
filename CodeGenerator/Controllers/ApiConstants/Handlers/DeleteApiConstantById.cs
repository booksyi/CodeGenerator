using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.ApiConstants.Handlers
{
    public class DeleteApiConstantById
    {
        public class Request : IRequest<uint>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, uint>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<uint> Handle(Request request, CancellationToken token)
            {
                DbApiConstant apiConstant = await context.ApiConstants.FirstOrDefaultAsync(x => x.Id == request.Id);
                context.ApiConstants.Remove(apiConstant);
                await context.SaveChangesAsync();
                return 0;
            }
        }
    }
}
