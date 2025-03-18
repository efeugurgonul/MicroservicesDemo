using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.CQRS.Queries
{
    public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : Query<TResponse>
    {
        public abstract Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken);
    }
}
