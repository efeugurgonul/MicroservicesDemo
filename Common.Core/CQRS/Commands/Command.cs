using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.CQRS.Commands
{
    public abstract class Command<TResponse> : IRequest<TResponse>
    {
    }
}
