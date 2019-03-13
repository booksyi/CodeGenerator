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
        public class Request : IRequest<CsSchemaClass>
        {
            public string ProgramText { get; set; }
        }

        public class Handler : IRequestHandler<Request, CsSchemaClass>
        {
            public Handler()
            {
            }

            public async Task<CsSchemaClass> Handle(Request request, CancellationToken token)
            {
                CsSchemaClass classSchema = CodingHelper.AnalysisClass(request.ProgramText);
                return classSchema;
            }
        }
    }
}
