using Microsoft.AspNetCore.Mvc;
using FidoDino.Domain.Entities.System;
using FidoDino.Application.Interfaces;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/system-status")]
    public class SystemStatusController : ControllerBase
    {
        private readonly ISystemStatusAppService _systemStatusService;
        public SystemStatusController(ISystemStatusAppService systemStatusService)
        {
            _systemStatusService = systemStatusService;
        }
        /// <summary>
        /// [2.3] Lấy trạng thái hệ thống
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCurrentStatus()
        {
            var status = await _systemStatusService.GetCurrentStatusAsync();
            return Ok(status);
        }

        /// <summary>
        /// [2.4] Cập nhật trạng thái hệ thống
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] SystemStatus status)
        {
            await _systemStatusService.UpdateStatusAsync(status);
            return NoContent();
        }
    }
}