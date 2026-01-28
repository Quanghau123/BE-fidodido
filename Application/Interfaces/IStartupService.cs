namespace FidoDino.Application.Interfaces
{
    public interface IStartupService
    {
        Task LoadPlatformDataToRedisAsync();
        Task<object> GetHealthStatusAsync();
    }
}