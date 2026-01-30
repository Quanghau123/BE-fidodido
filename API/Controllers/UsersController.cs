using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using FidoDino.Application.Interfaces;
using FidoDino.Application.DTOs.User;
using FidoDino.Common;
using FidoDino.Common.Authorization;

namespace FidoDino.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Policy = Permissions.User_Create)]
        public async Task<IActionResult> Register([FromBody] CreateUserRequest data)
        {
            var result = await _userService.CreateNewUserAsync(data);
            return Ok(new ApiResponse<object>(true, "User created successfully.", result));
        }

        [HttpGet]
        [Authorize(Policy = Permissions.User_View)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponse<object>(true, "Get users successfully.", users));
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = Permissions.User_Update)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest data)
        {
            var result = await _userService.HandleUpdateUserAsync(userId, data);
            return Ok(new ApiResponse<object>(true, "Update success.", result));
        }

        [HttpDelete("{userId}")]
        [Authorize(Policy = Permissions.User_Delete)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _userService.HandleDeleteUserAsync(userId);
            return Ok(new ApiResponse<object>(true, "Delete success.", result));
        }

        [HttpGet("export")]
        [Authorize(Policy = Permissions.User_Export)]
        public async Task<IActionResult> ExportUsersToStream([FromQuery] ExportUserRequest request, CancellationToken ct)
        {
            var stream = await _userService.ExportUsersToCsvAsync(request, ct);
            return File(stream, "text/csv", "users.csv");
        }

    }
}
