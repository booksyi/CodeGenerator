using CodeGenerator.Data.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.CodeTemplates.Handlers
{
    public class GetCodeTemplates
    {
        public class Request : IRequest<DbCodeTemplate[]>
        {
        }

        public class Handler : IRequestHandler<Request, DbCodeTemplate[]>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<DbCodeTemplate[]> Handle(Request request, CancellationToken token)
            {
                DbCodeTemplate[] codeTemplates = context.CodeTemplates.ToArray();
                return codeTemplates;
            }
        }
    }
}
