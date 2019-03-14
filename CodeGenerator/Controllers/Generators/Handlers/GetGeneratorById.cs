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
    public class GetGeneratorById
    {
        public class Request : IRequest<Generator>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, Generator>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Generator> Handle(Request request, CancellationToken token)
            {
                Generator generator = await context.Generators.FirstOrDefaultAsync(x => x.Id == request.Id);
                return generator;
            }
        }
    }
}
