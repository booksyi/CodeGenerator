using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.CodeTemplates.Handlers
{
    public class GetCodeTemplateById
    {
        public class Request : IRequest<DbCodeTemplate>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbCodeTemplate>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<DbCodeTemplate> Handle(Request request, CancellationToken token)
            {
                DbCodeTemplate codeTemplate = await context.CodeTemplates.FirstOrDefaultAsync(x => x.Id == request.Id);
                return codeTemplate;
            }
        }
    }
}
