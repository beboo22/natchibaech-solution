using Domain.Entity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace Travel_site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipReviewRequestController : ControllerBase
    {

        /**
         * GET api/MembershipReviewRequest/ : getall
         * PUT apiMembershipReviewRequest/id : update status
         */
        ApplicationDbContext _context;
        private readonly IQRCodeService _qrCodeService;


        public MembershipReviewRequestController(ApplicationDbContext context, IQRCodeService qrCodeService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok( new ApiResponse<List<MembershipReviewRequestDtos>>(200, await _context.MembershipReviewRequests.Select(_context => new MembershipReviewRequestDtos
            {
                UserId = _context.UserId,
                RequestedAt = _context.RequestedAt,
                Status = _context.Status,
            }).ToListAsync()));
        }

        [HttpPut("/{id:int}")]
        public async Task<IActionResult> UpdateStatus(int id,[FromBody] ReviewStatus status)
        {
            var item = await _context.MembershipReviewRequests
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item is null)
                return NotFound(new ApiResponse(404, "There's no MembershipReviewRequest by this id"));
            try
            {
                if(status == ReviewStatus.Approved)
                {
                    var existingCard = await _context.MembershipCards
                .FirstOrDefaultAsync(mc => mc.Id == item.MembershipCardId);

                    if (existingCard == null)
                    {
                        throw new InvalidOperationException("membership card Not found");

                    }

                    var membershipNumber = GenerateMembershipNumber(item.UserId);
                    var qrCodeData = GenerateMembershipQRData(membershipNumber, item.User.FullName, item.User.Category, DateTime.UtcNow.AddYears(1));
                    var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);


                    var membership = new MemberShip
                    {
                        MembershipCardId = item.MembershipCardId,
                        UserId = item.UserId,
                        MembershipNumber = membershipNumber,
                        BookingDate = DateTime.UtcNow,
                        Expiry = DateTime.UtcNow.AddYears(1), // Default 1 year validity
                        QrCode = qrCode,
                        Status = OrderStatus.Pending,
                    };
                    await _context.MemberShips.AddAsync(membership);
                }
                item.Status = status;
                _context.MembershipReviewRequests.Update(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the discount code", error = ex.Message });
            }
            return Ok(new ApiResponse(200));
        }





        private static string GenerateMembershipNumber(int userId)
        {
            return $"MEM{userId:D6}{DateTime.UtcNow.Year}";
        }

        private static string GenerateMembershipQRData(string membershipNumber, string memberName, UserType category, DateTime time)
        {
            return $"MEMBERSHIP:{membershipNumber}|NAME:{memberName}|CATEGORY:{category}|ISSUED:{DateTime.UtcNow:yyyy-MM-dd}|Expiry Data :{time:yyyy-MM-dd}";
        }


    }
}
