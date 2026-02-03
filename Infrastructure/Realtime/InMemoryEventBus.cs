using System.Collections.Concurrent;
using FidoDino.Application.Events;

namespace FidoDino.Infrastructure.Realtime;

public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, Type> _handlerTypes = new();

    public InMemoryEventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // Gửi một sự kiện đến tất cả các handler đã đăng ký cho loại sự kiện đó.
    //Mỗi lần publish sẽ tạo scope mới và resolve handler từ DI để tránh lỗi dispose DbContext.
    public async Task PublishAsync<TEvent>(TEvent evt)
    {
        if (_handlerTypes.TryGetValue(typeof(TEvent), out var handlerType))
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = (IEventHandler<TEvent>)scope.ServiceProvider
                .GetRequiredService(handlerType);

            await handler.HandleAsync(evt);
        }
    }

    // Đăng ký một handler cho một loại sự kiện cụ thể bằng cách lưu lại kiểu handler
    public void Subscribe<TEvent>(Type handlerType)
    {
        _handlerTypes[typeof(TEvent)] = handlerType;
    }
}
