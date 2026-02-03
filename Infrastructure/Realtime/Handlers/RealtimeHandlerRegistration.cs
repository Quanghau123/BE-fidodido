using FidoDino.Application.Events;

namespace FidoDino.Infrastructure.Realtime.Handlers
{
    public static class RealtimeHandlerRegistration
    {
        // Đăng ký các handler realtime với event bus, đảm bảo các sự kiện realtime sẽ được xử lý khi phát sinh.
        // Gọi hàm này sau khi DI đã khởi tạo đầy đủ.
        public static void RegisterHandlers(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<LeaderboardUpdatedEvent>(typeof(LeaderboardRealtimeHandler));
        }
    }
}
