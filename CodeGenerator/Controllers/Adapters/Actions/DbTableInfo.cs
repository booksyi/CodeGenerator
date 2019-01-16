﻿using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Actions
{
    public class DbTableInfo
    {
        public class Request : IRequest<object>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }

        public class Handler : IRequestHandler<Request, object>
        {
            public Handler()
            {
            }

            public async Task<object> Handle(Request request, CancellationToken token)
            {
                DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(request.ConnectionString, request.TableName);
                var primaryKeys = tableSchema.PrimaryKeys.Select(x => x.Name).ToArray();
                return new
                {
                    Identity = tableSchema.Identity,
                    PrimaryKeys = primaryKeys
                };
            }
        }
    }
}
