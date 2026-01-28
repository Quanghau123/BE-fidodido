using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Services;
using FidoDino.Application.Interfaces;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/startup")]
    public class StartupController : ControllerBase
    {
        private readonly IStartupService _startupService;
        public StartupController(IStartupService startupService)
        {
            _startupService = startupService;
        }

        /// <summary>
        /// [2.2] Quản lý Khởi động và Hệ thống:
        /// Kiểm tra trạng thái sẵn sàng và mức độ đồng bộ dữ liệu Redis
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            var result = await _startupService.GetHealthStatusAsync();
            return Ok(result);
        }
    }
}