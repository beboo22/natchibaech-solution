using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        /// <summary>
        /// Apply discount code to order
        /// </summary>
        [HttpPost("apply")]
        public async Task<ActionResult<DiscountResultDto>> ApplyDiscount([FromBody] ApplyDiscountDto applyDiscountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //if (discountCode == null)
                //    return BadRequest(ModelState);
                //if (discountCode.Code != applyDiscountDto.Code)
                //    return BadRequest(new ApiResponse(404, "not found code"));
                var discountAmount = await _discountService.ApplyDiscountAsync(
                    applyDiscountDto.Code,
                    applyDiscountDto.OrderId);
                var discountCode = await _discountService.GetDiscountCodeByCodeAsync(applyDiscountDto.Code);

                var finalAmount = applyDiscountDto.Amount - discountAmount;

                var result = new DiscountResultDto
                {
                    Success = true,
                    Message = "Discount applied successfully",
                    OriginalAmount = applyDiscountDto.Amount,
                    DiscountAmount = discountAmount,
                    DiscountPercentage = discountCode?.Percentage ?? 0,
                    DiscountCode = applyDiscountDto.Code
                };

                return Ok(new ApiResponse<DiscountResultDto>(200, result));
            }
            catch (ArgumentException ex)
            {
                var result = new DiscountResultDto
                {
                    Success = false,
                    Message = ex.Message,
                    OriginalAmount = applyDiscountDto.Amount,
                    DiscountAmount = 0,
                    DiscountPercentage = 0,
                    DiscountCode = applyDiscountDto.Code
                };
                //what code should I return here?

                return BadRequest(new ApiResponse<DiscountResultDto>(400, result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while applying the discount", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all discount codes (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountCodeDto>>> GetDiscountCodes()
        {
            try
            {
                var discountCodes = await _discountService.GetAllDiscountCodesAsync();
                var discountCodeDtos = discountCodes.Select(MapToDiscountCodeDto);

                return Ok(new ApiResponse<IEnumerable<DiscountCodeDto>>(200, discountCodeDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving discount codes", error = ex.Message });
            }
        }

        /// <summary>
        /// Get discount code by code
        /// </summary>
        [HttpGet("{code}")]
        public async Task<ActionResult<DiscountCodeDto>> GetDiscountCode(string code)
        {
            try
            {
                var discountCode = await _discountService.GetDiscountCodeByCodeAsync(code);
                if (discountCode == null)
                    return NotFound(new { message = "Discount code not found" });

                var discountCodeDto = MapToDiscountCodeDto(discountCode);
                return Ok(new ApiResponse<DiscountCodeDto>(200, discountCodeDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the discount code", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new discount code (Admin only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DiscountCodeDto>> CreateDiscountCode([FromBody] CreateDiscountCodeDto createDiscountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var discountCode = new DiscountCode
                {
                    MemberShipId = createDiscountDto.MemberId,
                    UserId = createDiscountDto.UserId,
                    Code = createDiscountDto.Code.ToUpper(),
                    Percentage = createDiscountDto.Percentage,
                    MaxUsage = createDiscountDto.MaxUsage,
                    ExpiryDate = createDiscountDto.ExpiryDate,
                    CurrentUsage = 0,
                    IsActive = true
                };

                var createdDiscountCode = await _discountService.CreateDiscountCodeAsync(discountCode);
                var discountCodeDto = MapToDiscountCodeDto(createdDiscountCode);
                // use ApiResponse to return the created discount code
                if (discountCodeDto == null)
                    return BadRequest(new ApiResponse(400, "Discount code could not be created"));
                return Ok(new ApiResponse<DiscountCodeDto>(201, discountCodeDto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse(500, ex.Message));
            }
        }

        /// <summary>
        /// Update discount code (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DiscountCodeDto>> UpdateDiscountCode(int id, [FromBody] CreateDiscountCodeDto updateDiscountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var discountCode = new DiscountCode
                {
                    UserId = updateDiscountDto.UserId,
                    Code = updateDiscountDto.Code.ToUpper(),
                    Percentage = updateDiscountDto.Percentage,
                    MaxUsage = updateDiscountDto.MaxUsage,
                    ExpiryDate = updateDiscountDto.ExpiryDate,
                    IsActive = updateDiscountDto.IsActive
                };

                var updatedDiscountCode = await _discountService.UpdateDiscountCodeAsync(id, discountCode);
                if (updatedDiscountCode == null)
                    return NotFound(new { message = "Discount code not found" });

                var discountCodeDto = MapToDiscountCodeDto(updatedDiscountCode);
                return Ok(new ApiResponse<DiscountCodeDto>(200, discountCodeDto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the discount code", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete discount code (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDiscountCode(int id)
        {
            try
            {
                var result = await _discountService.DeleteDiscountCodeAsync(id);
                if (!result)
                    return NotFound(new { message = "Discount code not found" });

                return Ok(new { message = "Discount code deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the discount code", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate discount code without applying it
        /// </summary>
        [HttpPost("validate")]
        public async Task<ActionResult<object>> ValidateDiscountCode([FromBody] string code)
        {
            try
            {
                var isValid = await _discountService.ValidateDiscountCodeAsync(code);

                if (isValid)
                {
                    var discountCode = await _discountService.GetDiscountCodeByCodeAsync(code);
                    return Ok(new
                    {
                        valid = true,
                        message = "Discount code is valid",
                        percentage = discountCode?.Percentage ?? 0,
                        remainingUsage = (discountCode?.MaxUsage ?? 0) - (discountCode?.CurrentUsage ?? 0)
                    });
                }
                else
                {
                    return Ok(new { valid = false, message = "Invalid or expired discount code" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating the discount code", error = ex.Message });
            }
        }

        private static DiscountCodeDto MapToDiscountCodeDto(DiscountCode discountCode)
        {
            return new DiscountCodeDto
            {
                Id = discountCode.Id,
                UserId = discountCode.UserId,
                Code = discountCode.Code,
                Percentage = discountCode.Percentage,
                MaxUsage = discountCode.MaxUsage,
                CurrentUsage = discountCode.CurrentUsage,
                ExpiryDate = discountCode.ExpiryDate,//discountCode.ExpiryDate > DateTime.UtcNow && discountCode.CurrentUsage < discountCode.MaxUsage,
                
                IsActive = discountCode.IsActive,
                RemainingUsage = Math.Max(0, discountCode.MaxUsage - discountCode.CurrentUsage)
            };
        }
    }
}
