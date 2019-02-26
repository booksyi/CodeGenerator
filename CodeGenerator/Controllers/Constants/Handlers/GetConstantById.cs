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
    public class GetConstantById
    {
        public class Request : IRequest<Constant>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, Constant>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Constant> Handle(Request request, CancellationToken token)
            {
                Constant constant = await context.Constants.FirstOrDefaultAsync(x => x.Id == request.Id);
                return constant;
            }
        }
    }
}
