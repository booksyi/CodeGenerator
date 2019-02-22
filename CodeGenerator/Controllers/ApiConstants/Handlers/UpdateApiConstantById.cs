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
    public class UpdateApiConstantById
    {
        public class Request : IRequest<DbApiConstant>
        {
            internal int Id { get; set; }
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
                DbApiConstant apiConstant = await context.ApiConstants.FirstOrDefaultAsync(x => x.Id == request.Id);
                apiConstant.Result = request.Result;
                await context.SaveChangesAsync();
                return apiConstant;
            }
        }
    }
}
