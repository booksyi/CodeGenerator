using CodeGenerator.Data;
using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Handlers
{
    public class DeleteGeneratorById
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
                Generator generator = await context.Generators.FirstOrDefaultAsync(x => x.Id == request.Id);
                context.Generators.Remove(generator);
                await context.SaveChangesAsync();
                return 0;
            }
        }
    }
}
