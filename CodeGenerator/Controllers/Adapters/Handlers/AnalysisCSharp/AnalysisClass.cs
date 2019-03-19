using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Handlers.AnalysisCSharp
{
    public class AnalysisClass
    {
        public class Request : IRequest<CsSchema.Class>
        {
            public string ProgramText { get; set; }
        }

        public class Handler : IRequestHandler<Request, CsSchema.Class>
        {
            public Handler()
            {
            }

            public async Task<CsSchema.Class> Handle(Request request, CancellationToken token)
            {
                CsSchema.Class classSchema = CodingHelper.AnalysisClass(request.ProgramText);
                return classSchema;
            }
        }
    }
}
