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
    public class CreateOrUpdateConstant
    {
        public class Request : IRequest<Constant>
        {
            internal int Id { get; set; }
            public string Result { get; set; }
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
                Constant constant;
                if (request.Id == 0)
                {
                    constant = new Constant()
                    {
                        Result = request.Result
                    };
                    await context.Constants.AddAsync(constant);
                }
                else
                {
                    constant = await context.Constants.FirstOrDefaultAsync(x => x.Id == request.Id);
                    constant.Result = request.Result;
                }
                await context.SaveChangesAsync();
                return constant;
            }
        }
    }
}
