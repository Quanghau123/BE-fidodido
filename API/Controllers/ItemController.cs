using Microsoft.AspNetCore.Mvc;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Game;
using FidoDino.Common;
using System;
using System.Threading.Tasks;
using FidoDino.Application.DTOs.Game;

namespace FidoDino.API.Controllers
{
    [ApiController]
    [Route("api/v1/items")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// [6.1] Lấy 1 ice
        /// </summary>
        [HttpGet("ice/{id}")]
        public async Task<IActionResult> GetIceById(Guid id)
        {
            var result = await _itemService.GetIceByIdAsync(id);
            return Ok(new ApiResponse<Ice>(true, "Success", result));
        }

        /// <summary>
        /// [6.2] Lấy tất cả ice
        /// </summary>
        [HttpGet("ice")]
        public async Task<IActionResult> GetAllIce()
        {
            var result = await _itemService.GetAllIceAsync();
            return Ok(new ApiResponse<IEnumerable<Ice>>(true, "Success", result));
        }

        /// <summary>
        /// [6.3] Thêm ice
        /// </summary>
        [HttpPost("ice")]
        public async Task<IActionResult> AddIce([FromBody] IceRequestAdd ice)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            await _itemService.AddIceAsync(ice);
            return Ok(new ApiResponse<object>(true, "Ice added successfully"));
        }

        /// <summary>
        /// [6.4] Sửa ice
        /// </summary>
        [HttpPut("ice")]
        public async Task<IActionResult> UpdateIce([FromBody] IceRequestUpdate ice)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            await _itemService.UpdateIceAsync(ice);
            return Ok(new ApiResponse<object>(true, "Ice updated successfully"));
        }

        /// <summary>
        /// [6.5] Xóa ice
        /// </summary>
        [HttpDelete("ice/{id}")]
        public async Task<IActionResult> DeleteIce(Guid id)
        {
            await _itemService.DeleteIceAsync(id);
            return Ok(new ApiResponse<object>(true, "Ice deleted successfully"));
        }

        // Reward endpoints
        /// <summary>
        /// [6.6] Lấy 1 reward
        /// </summary>
        [HttpGet("reward/{id}")]
        public async Task<IActionResult> GetRewardById(Guid id)
        {
            var result = await _itemService.GetRewardByIdAsync(id);
            return Ok(new ApiResponse<Reward>(true, "Success", result));
        }

        /// <summary>
        /// [6.7] Lấy tất cả reward
        /// </summary>
        [HttpGet("reward")]
        public async Task<IActionResult> GetAllReward()
        {
            var result = await _itemService.GetAllRewardAsync();
            return Ok(new ApiResponse<IEnumerable<Reward>>(true, "Success", result));
        }

        /// <summary>
        /// [6.8] Thêm reward
        /// </summary>
        [HttpPost("reward")]
        public async Task<IActionResult> AddReward([FromBody] RewardRequestAdd reward)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            await _itemService.AddRewardAsync(reward);
            return Ok(new ApiResponse<object>(true, "Reward added successfully"));
        }

        /// <summary>
        /// [6.9] Sửa reward
        /// </summary>
        [HttpPut("reward")]
        public async Task<IActionResult> UpdateReward([FromBody] RewardRequestUpdate reward)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            await _itemService.UpdateRewardAsync(reward);
            return Ok(new ApiResponse<object>(true, "Reward updated successfully"));
        }

        /// <summary>
        /// [6.10] Xóa reward
        /// </summary>
        [HttpDelete("reward/{id}")]
        public async Task<IActionResult> DeleteReward(Guid id)
        {
            await _itemService.DeleteRewardAsync(id);
            return Ok(new ApiResponse<object>(true, "Reward deleted successfully"));
        }

        // IceReward endpoints
        /// <summary>
        /// [6.11] Lấy iceReward theo ice
        /// </summary>
        [HttpGet("icereward/ice/{iceId}")]
        public async Task<IActionResult> GetIceRewardsByIceId(Guid iceId)
        {
            var result = await _itemService.GetIceRewardsByIceIdAsync(iceId);
            return Ok(new ApiResponse<IEnumerable<IceReward>>(true, "Success", result));
        }

        /// <summary>
        /// [6.12] Thêm iceReward
        /// </summary>
        [HttpPost("icereward")]
        public async Task<IActionResult> AddIceReward([FromBody] IceRewardRequestAdd iceReward)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            await _itemService.AddIceRewardAsync(iceReward);
            return Ok(new ApiResponse<object>(true, "IceReward added successfully"));
        }

        /// <summary>
        /// [6.13] Sửa iceReward
        /// </summary>
        [HttpPut("icereward")]
        public async Task<IActionResult> UpdateIceReward([FromBody] IceRewardRequestUpdate iceReward)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(false, "Invalid data", ModelState));
            await _itemService.UpdateIceRewardAsync(iceReward);
            return Ok(new ApiResponse<object>(true, "IceReward updated successfully"));
        }

        /// <summary>
        /// [6.14] Xóa iceReward
        /// </summary>
        [HttpDelete("icereward/{id}")]
        public async Task<IActionResult> DeleteIceReward(Guid id)
        {
            await _itemService.DeleteIceRewardAsync(id);
            return Ok(new ApiResponse<object>(true, "IceReward deleted successfully"));
        }
    }
}
