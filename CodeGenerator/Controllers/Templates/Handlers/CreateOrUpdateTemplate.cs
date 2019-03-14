using CodeGenerator.Data;
using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Templates.Handlers
{
    public class CreateOrUpdateTemplate
    {
        public class Request : IRequest<Template>
        {
            internal int Id { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
        }

        public class Handler : IRequestHandler<Request, Template>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Template> Handle(Request request, CancellationToken token)
            {
                Template template;
                if (request.Id == 0)
                {
                    template = new Template()
                    {
                        Name = request.Name,
                        Content = request.Content,
                        CreateDate = DateTime.Now
                    };
                    await context.Templates.AddAsync(template);
                }
                else
                {
                    template = await context.Templates.FirstOrDefaultAsync(x => x.Id == request.Id);
                    template.Name = request.Name;
                    template.Content = request.Content;
                    template.UpdateDate = DateTime.Now;
                }
                await context.SaveChangesAsync();
                return template;

            }
        }
    }
}
