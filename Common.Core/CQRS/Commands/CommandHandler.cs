using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.CQRS.Commands
{
    public abstract class CommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : Command<TResponse>
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
    }
}
