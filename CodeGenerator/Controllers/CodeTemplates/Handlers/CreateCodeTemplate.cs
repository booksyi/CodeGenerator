using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.CodeTemplates.Handlers
{
    public class CreateCodeTemplate
    {
        public class Request : IRequest<DbCodeTemplate>
        {
            public CodeTemplate Template { get; set; }
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
                DbCodeTemplate codeTemplate = new DbCodeTemplate()
                {
                    Node = JsonConvert.SerializeObject(request.Template)
                };
                await context.CodeTemplates.AddAsync(codeTemplate);
                await context.SaveChangesAsync();
                return codeTemplate;
            }
        }
    }
}
