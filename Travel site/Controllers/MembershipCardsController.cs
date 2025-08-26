using Domain.Entity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/membership")]
    public class MembershipCardsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IDiscountService _discountService;
        private ApplicationDbContext _context;

        public MembershipCardsController(IMembershipService membershipService, IDiscountService discountService, ApplicationDbContext context)
        {
            _membershipService = membershipService;
            _discountService = discountService;
            _context = context;
        }

        [HttpPost("CreateMembershipCard")]
        public async Task<IActionResult> CreateMembershipCard(MembershipCardDto entity)
        {
            try
            {
                await _context.MembershipCards.AddAsync(new MembershipCard
                {
                    IsActive = entity.IsActive,
                    Price = entity.Price,
                });
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(200));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }



        /// <summary>
        /// Issue membership card to user
        /// </summary>
        [HttpPost("issue/")]
        public async Task<ActionResult> IssueMembershipCard([FromBody] IssueMembershipCardDto issueDto)
        {
            try
            {
                var membershipCard = await _membershipService.IssueMembershipAsync(issueDto);



                var membershipCardDto = await MapToMembershipCardDtoAsync(membershipCard);
                return Ok(new ApiResponse<MembershipDto>(200, membershipCardDto));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while issuing the membership card", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user's membership card
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetMembershipCard(int userId)
        {
            try
            {
                var membershipCard = await _membershipService.GetMembershipAsync(userId);
                if (membershipCard == null)
                    return NotFound(new { message = "Membership card not found" });

                var membershipCardDto = await MapToMembershipCardDtoAsync(membershipCard);
                return Ok(new ApiResponse<MembershipDto>(200, membershipCardDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the membership card", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all membership cards (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetAllMembershipCards([FromQuery] bool activeOnly = false)
        {
            try
            {
                var membershipCards = activeOnly
                    ? await _membershipService.GetActiveMembershipAsync()
                    : await _membershipService.GetAllMembershipAsync();

                var membershipCardDtos = new List<MembershipDto>();
                foreach (var card in membershipCards)
                {
                    membershipCardDtos.Add(await MapToMembershipCardDtoAsync(card));
                }

                return Ok(new ApiResponse<List<MembershipDto>>(200, membershipCardDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving membership cards", error = ex.Message });
            }
        }

        /// <summary>
        /// Get membership cards expiring soon (Admin only)
        /// </summary>
        [HttpGet("expiring")]
        public async Task<ActionResult> GetExpiringMembershipCards([FromQuery] int days = 30)
        {
            try
            {
                var membershipCards = await _membershipService.GetExpiringMembershipAsync(days);

                var membershipCardDtos = new List<MembershipDto>();
                foreach (var card in membershipCards)
                {
                    membershipCardDtos.Add(await MapToMembershipCardDtoAsync(card));
                }
                return Ok(new ApiResponse<List<MembershipDto>>(200, membershipCardDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving expiring membership cards", error = ex.Message });
            }
        }

        /// <summary>
        /// Update membership card details (Admin only)
        /// </summary>
        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateMembershipCard(int userId, [FromBody] UpdateMembershipDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var membershipCard = await _membershipService.UpdateMembershipAsync(
                    userId, updateDto);

                if (membershipCard == null)
                    return NotFound(new { message = "Membership card not found" });

                var membershipCardDto = await MapToMembershipCardDtoAsync(membershipCard);
                return Ok(new ApiResponse<MembershipDto>(200, membershipCardDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the membership card", error = ex.Message });
            }
        }

        /// <summary>
        /// Renew membership card
        /// </summary>
        [HttpPost("{userId}/renew")]
        public async Task<ActionResult> RenewMembershipCard(int userId, [FromQuery] int months = 12)
        {
            try
            {
                var success = await _membershipService.RenewMembershipAsync(userId, months);
                if (!success)
                    return NotFound(new { message = "Membership card not found" });

                return Ok(new { message = $"Membership card renewed for {months} months" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while renewing the membership card", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate membership card
        /// </summary>
        [HttpPost("validate")]
        public async Task<ActionResult> ValidateMembershipCard([FromBody] string membershipNumber)
        {
            try
            {
                var isValid = await _membershipService.ValidateMembershipAsync(membershipNumber);

                if (isValid)
                {
                    return Ok(new MembershipCardValidationDto
                    {
                        IsValid = true,
                        Message = "Membership card is valid",
                        ValidationCode = "VALID"
                    });
                }
                else
                {
                    return Ok(new MembershipCardValidationDto
                    {
                        IsValid = false,
                        Message = "Membership card is invalid or expired",
                        ValidationCode = "INVALID"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating the membership card", error = ex.Message });
            }
        }

        /// <summary>
        /// Revoke membership card (Admin only)
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<ActionResult> RevokeMembershipCard(int userId)
        {
            try
            {
                var success = await _membershipService.RevokeMembershipAsync(userId);
                if (!success)
                    return NotFound(new { message = "Membership card not found" });

                return Ok(new { message = "Membership card revoked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while revoking the membership card", error = ex.Message });
            }
        }

        /// <summary>
        /// Get membership statistics (Admin only)
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult> GetMembershipStats()
        {
            try
            {
                var stats = await _membershipService.GetMembershipStatsAsync();
                return Ok(new ApiResponse<MembershipStatsDto>(200,stats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving membership statistics", error = ex.Message });
            }
        }

        private async Task<MembershipDto> MapToMembershipCardDtoAsync(MemberShip membershipCard)
        {
            // Get available discount codes for this membership card
            var discountCodes = await _discountService.GetAllDiscountCodesAsync();
            var availableDiscounts = discountCodes
                .Where(dc => dc.UserId == membershipCard.UserId || dc.UserId == null)
                .Where(dc => dc.ExpiryDate > DateTime.UtcNow && dc.CurrentUsage < dc.MaxUsage)
                .Select(dc => new DiscountCodeDto
                {
                    Id = dc.Id,
                    Code = dc.Code,
                    Percentage = dc.Percentage,
                    MaxUsage = dc.MaxUsage,
                    CurrentUsage = dc.CurrentUsage,
                    ExpiryDate = dc.ExpiryDate,
                    IsActive = dc.ExpiryDate > DateTime.UtcNow && dc.CurrentUsage < dc.MaxUsage,
                    RemainingUsage = Math.Max(0, dc.MaxUsage - dc.CurrentUsage)
                })
                .ToList();

            return new MembershipDto
            {
                Id = membershipCard.Id,
                UserId = membershipCard.UserId,
                UserName = membershipCard.User?.FullName ?? string.Empty,
                UserEmail = membershipCard.User?.Email ?? string.Empty,
                UserCategory = membershipCard.User?.Category ?? UserCategory.Men,
                BookingDate = membershipCard.BookingDate,
                Expiry = membershipCard.Expiry,
                QrCode = membershipCard.QrCode,
                IsActive = membershipCard.Expiry > DateTime.UtcNow,
                DaysUntilExpiry = (int)(membershipCard.Expiry - DateTime.UtcNow).TotalDays,
                MembershipNumber = $"MEM{membershipCard.UserId:D6}{membershipCard.BookingDate.Year}",
                AvailableDiscounts = availableDiscounts,
                Price = membershipCard.MembershipCard?.Price??0,
                status = membershipCard.Status                
            };
        }
    }
}
