using System.Threading.Tasks;

namespace FidoDino.Application.Events
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent evt);
        void Subscribe<TEvent>(Type handlerType);
    }
}
