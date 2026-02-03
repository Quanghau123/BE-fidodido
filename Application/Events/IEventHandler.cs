using System;
using System.Threading.Tasks;

namespace FidoDino.Application.Events
{
    public interface IEventHandler<TEvent>
    {
        Task HandleAsync(TEvent evt);
    }
}
