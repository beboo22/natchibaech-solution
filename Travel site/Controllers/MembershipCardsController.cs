using Domain.Entity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/membership")]
    public class MembershipCardsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IOrderDiscountService _discountService;
        private ApplicationDbContext _context;

        public MembershipCardsController(IMembershipService membershipService, IOrderDiscountService discountService, ApplicationDbContext context)
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
                    ServiceCategory = entity.ServiceCategory,
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
        [HttpGet("MembershipCard")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MembershipCard>>>> GetMembershipCard()
        {
            try
            {
                return Ok(new ApiResponse<IEnumerable<MembershipCard>>(200, await _context.MembershipCards.ToListAsync()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }
        [HttpPut("UpdateMembershipCard/{id}")]
        public async Task<IActionResult> UpdateMembershipCard(int id, MembershipCardDto entity)
        {
            if (entity == null)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Invalid membership card data"));
            }

            try
            {
                var item = await _context.MembershipCards.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, $"Membership card with ID {id} not found"));
                }

                // Update existing entity instead of creating new
                item.IsActive = entity.IsActive;
                item.Price = entity.Price;

                // Optional: Add validation
                if (entity.Price < 0)
                {
                    return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Price cannot be negative"));
                }

                _context.MembershipCards.Update(item);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse((int)HttpStatusCode.OK, "Membership card updated successfully"));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new ApiResponse((int)HttpStatusCode.Conflict, "Membership card was modified by another user"));
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiResponse((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteMembershipCard/{id}")]
        public async Task<IActionResult> DeleteMembershipCard(int id)
        {
            try
            {
                var item = _context.MembershipCards.Find(id);
                if (item == null)
                    return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "there's no Membershipcard"));

                _context.MembershipCards.Remove(item);
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
        public async Task<ActionResult<ApiResponse<MembershipDto>>> IssueMembershipCard([FromBody] IssueMembershipCardDto issueDto)
        {
            try
            {
                var membershipCard = await _membershipService.IssueMembershipAsync(issueDto);



                var membershipCardDto =  MapToMembershipCardDtoAsync(membershipCard);
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
        [HttpGet("{Email}")]
        public async Task<ActionResult> GetMembershipCard(string Email)
        {
            try
            {
                var membershipCard = await _membershipService.GetMembershipAsync(Email);
                if (membershipCard == null)
                    return NotFound(new { message = "Membership card not found" });

                var membershipCardDto = MapToMembershipCardDtoAsync(membershipCard);
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
        public async Task<ActionResult<List<MembershipDto>>> GetAllMembershipCards([FromQuery] bool activeOnly = false)
        {
            try
            {
                var membershipCards = activeOnly
                    ? await _membershipService.GetActiveMembershipAsync()
                    : await _membershipService.GetAllMembershipAsync();

                var membershipCardDtos = new List<MembershipDto>();
                foreach (var card in membershipCards)
                {
                    membershipCardDtos.Add(MapToMembershipCardDtoAsync(card));
                }
    //            var membershipCardDtos = await Task.WhenAll(
    //membershipCards.Select(m => MapToMembershipCardDtoAsync(m))
//);

                return Ok(new ApiResponse<List<MembershipDto>>(200, membershipCardDtos.ToList()));

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
                    membershipCardDtos.Add(MapToMembershipCardDtoAsync(card));
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
        [HttpPut("{Email}")]
        public async Task<ActionResult> UpdateMembershipCard(string Email, [FromBody] UpdateMembershipDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var membershipCard = await _membershipService.UpdateMembershipAsync(
                    Email, updateDto);

                if (membershipCard == null)
                    return NotFound(new { message = "Membership card not found" });

                var membershipCardDto = MapToMembershipCardDtoAsync(membershipCard);
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
        [HttpPost("{Email}/renew")]
        public async Task<ActionResult> RenewMembershipCard(string Email, [FromQuery] int months = 12)
        {
            try
            {
                var success = await _membershipService.RenewMembershipAsync(Email, months);
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
        [HttpDelete("{Email}")]
        public async Task<ActionResult> RevokeMembershipCard(string Email)
        {
            try
            {
                var success = await _membershipService.RevokeMembershipAsync(Email);
                if (!success)
                    return NotFound(new { message = "Membership  not found" });

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
                return Ok(new ApiResponse<MembershipStatsDto>(200, stats));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving membership statistics", error = ex.Message });
            }
        }

        private MembershipDto MapToMembershipCardDtoAsync(MemberShip membershipCard)
        {
            //var memberShipsItms = await _membershipService.GetAllMembershipAsync();
            //var availableDiscounts = memberShipsItms.Where(c => c.Id == membershipCard.Id&& c.MemberShipDiscountCode is not null)?.Select(dc => new DiscountCodeDto
            //{
            //    Id = dc.MemberShipDiscountCode.Id,
            //    Code = dc.MemberShipDiscountCode.Code,
            //    Percentage = dc.MemberShipDiscountCode.Percentage,
            //    MaxUsage = dc.MemberShipDiscountCode.MaxUsage,
            //    CurrentUsage = dc.MemberShipDiscountCode.CurrentUsage,
            //    ExpiryDate = dc.MemberShipDiscountCode.ExpiryDate,
            //    IsActive = dc.MemberShipDiscountCode.ExpiryDate > DateTime.UtcNow && dc.MemberShipDiscountCode.CurrentUsage < dc.MemberShipDiscountCode.MaxUsage,
            //    RemainingUsage = Math.Max(0, dc.MemberShipDiscountCode.MaxUsage - dc.MemberShipDiscountCode.CurrentUsage)
            //});

            return new MembershipDto
            {
                Id = membershipCard.Id,
                UserId = membershipCard.UserId,
                UserName = membershipCard.User?.FullName ?? string.Empty,
                UserEmail = membershipCard.User?.Email ?? string.Empty,
                UserPhone = membershipCard.User.Phone ?? string.Empty,
                Ssn = membershipCard.User?.Ssn??string.Empty,
                BookingDate = membershipCard.BookingDate,
                Expiry = membershipCard.Expiry,
                QrCode = membershipCard.QrCode,
                IsActive = membershipCard.Expiry > DateTime.UtcNow,
                DaysUntilExpiry = (int)(membershipCard.Expiry - DateTime.UtcNow).TotalDays,
                MembershipNumber = $"MEM{membershipCard.UserId:D6}{membershipCard.BookingDate.Year}",
                //AvailableDiscounts = availableDiscounts is not null ? availableDiscounts.FirstOrDefault(): new DiscountCodeDto(),
                Price = membershipCard.MembershipCard?.Price ?? 0,
                status = membershipCard.Status,
                PartnerPhone = membershipCard.PartnerPhone,
                PartnerName = membershipCard.PartnerName,
                PartnerEmail = membershipCard.PartnerEmail,
            };
        }
    }
}
