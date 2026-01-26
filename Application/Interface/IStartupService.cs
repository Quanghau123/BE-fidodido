namespace FidoDino.Application.Interface
{
    public interface IStartupService
    {
        Task LoadPlatformDataToRedisAsync();
        Task<object> GetHealthStatusAsync();
    }
}