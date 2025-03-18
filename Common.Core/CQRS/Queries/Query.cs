using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.CQRS.Queries
{
    public abstract class Query<TResponse> : IRequest<TResponse>
    {
    }
}
