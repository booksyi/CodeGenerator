using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Templates.Handlers
{
    public class CreateTemplate
    {
        public class Request : IRequest<Template>
        {
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
                try
                {
                    Template template = new Template()
                    {
                        Name = request.Name,
                        Content = request.Content,
                        CreateDate = DateTime.Now
                    };
                    await context.Templates.AddAsync(template);
                    await context.SaveChangesAsync();
                    return template;
                }
                catch (Exception e)
                {
                    string a = e.Message;
                }
                return null;
            }
        }
    }
}
