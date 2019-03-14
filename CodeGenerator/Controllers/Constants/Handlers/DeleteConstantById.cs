using CodeGenerator.Data;
using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Constants.Handlers
{
    public class DeleteConstantById
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
                Constant constant = await context.Constants.FirstOrDefaultAsync(x => x.Id == request.Id);
                context.Constants.Remove(constant);
                await context.SaveChangesAsync();
                return 0;
            }
        }
    }
}
