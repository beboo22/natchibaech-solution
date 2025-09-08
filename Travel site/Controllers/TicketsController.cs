using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Generate tickets for order after payment (with QR codes)
        /// </summary>
        [HttpPost("generate/{orderId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GenerateTickets(int orderId, [FromBody] GenerateTicketsDto? generateDto = null)
        {
            try
            {
                var tickets = await _ticketService.GenerateTicketsAsync(orderId);
                var ticketDtos = tickets.Select(MapToTicketDto);

                // Optionally send tickets immediately
                if (generateDto?.DeliveryMethod != TicketDelivery.None)
                {
                    foreach (var ticket in tickets)
                    {
                        await _ticketService.SendTicketAsync(ticket.Id, generateDto?.DeliveryMethod ?? TicketDelivery.Email);
                    }
                }
                //use ApiResponse to return the list of tickets
                if (!ticketDtos.Any())
                    return NotFound(new { message = "No tickets generated" });
                return Ok(new ApiResponse<IEnumerable<TicketDto>>(200, ticketDtos, "Tickets generated successfully"));
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
                return StatusCode(500, new { message = "An error occurred while generating tickets", error = ex.Message });
            }
        }
        [HttpPost("generateMemberShioTickets/{MemberId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GenerateMemberShipTickets(int MemberId, [FromBody] GenerateTicketsDto? generateDto = null)
        {
            try
            {
                var tickets = await _ticketService.GenerateMemberTicketsAsync(MemberId);
                var ticketDtos = MapToTicketDto(tickets);

                if (ticketDtos is not null)
                {

                    // Optionally send tickets immediately
                    if (generateDto?.DeliveryMethod != TicketDelivery.None)
                    {
                        await _ticketService.SendTicketAsync(ticketDtos.Id, generateDto?.DeliveryMethod ?? TicketDelivery.Email);

                    }
                    return Ok(new ApiResponse<TicketDto>(200, ticketDtos, "Tickets generated successfully"));
                }
                //use ApiResponse to return the list of tickets
                return NotFound(new { message = "No tickets generated" });
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
                return StatusCode(500, new { message = "An error occurred while generating tickets", error = ex.Message });
            }
        }

        /// <summary>
        /// Get ticket details by ticket number
        /// </summary>
        [HttpGet("{ticketNumber}")]
        public async Task<ActionResult> GetTicket(string ticketNumber)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);
                if (ticket == null)
                    return NotFound(new { message = "Ticket not found" });

                var ticketDto = MapToTicketDto(ticket);
                //use ApiResponse to return the ticket details
                if (ticketDto == null)
                    return NotFound(new { message = "Ticket details not found" });
                return Ok(new ApiResponse<TicketDto>(200, ticketDto, "Ticket retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all tickets for a specific user
        /// </summary>
        [HttpGet("user/{Email}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetUserTickets(string Email)
        {
            try
            {
                var tickets = await _ticketService.GetUserTicketsAsync(Email);
                var ticketDtos = tickets.Select(MapToTicketDto);
                if (!ticketDtos.Any())
                    return NotFound(new { message = "No tickets found for this user" });
                //use ApiResponse to return the list of user tickets
                return Ok(new ApiResponse<IEnumerable<TicketDto>>(200, ticketDtos, "User tickets retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user tickets", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all tickets for a specific order
        /// </summary>
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetOrderTickets(int orderId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByOrderAsync(orderId);
                var ticketDtos = tickets.Select(MapToTicketDto);
                if (!ticketDtos.Any())
                    return NotFound(new { message = "No tickets found for this order" });
                //use ApiResponse to return the list of order tickets
                return Ok(new ApiResponse<IEnumerable<TicketDto>>(200, ticketDtos, "Order tickets retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving order tickets", error = ex.Message });
            }
        }

        /// <summary>
        /// Send ticket via Email/WhatsApp
        /// </summary>
        [HttpPost("{ticketId}/send")]
        public async Task<ActionResult> SendTicket(int ticketId, [FromBody] SendTicketDto sendTicketDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _ticketService.SendTicketAsync(ticketId, sendTicketDto.DeliveryMethod, sendTicketDto.CustomEmail);

                if (!success)
                    return BadRequest(new { message = "Failed to send ticket" });

                return Ok(new { message = "Ticket sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate ticket by ticket number
        /// </summary>
        [HttpGet("validate/{ticketNumber}")]
        public async Task<ActionResult<TicketValidationDto>> ValidateTicket(string ticketNumber)
        {
            try
            {
                var isValid = await _ticketService.ValidateTicketAsync(ticketNumber);

                if (isValid)
                {
                    var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);
                    return Ok(new TicketValidationDto
                    {
                        IsValid = true,
                        Message = "Ticket is valid",
                        Ticket = ticket != null ? MapToTicketDto(ticket) : null,
                        ValidationCode = "VALID"
                    });
                }
                else
                {
                    return Ok(new TicketValidationDto
                    {
                        IsValid = false,
                        Message = "Ticket is invalid or expired",
                        ValidationCode = "INVALID"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating the ticket", error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk generate tickets for multiple people
        /// </summary>
        [HttpPost("bulk-generate")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> BulkGenerateTickets([FromBody] BulkTicketGenerationDto bulkDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // For now, use the existing generate method
                // In a real implementation, you might want to create a specialized bulk method
                var tickets = await _ticketService.GenerateTicketsAsync(bulkDto.OrderId);
                var ticketDtos = tickets.Select(MapToTicketDto);
                //use ApiResponse to return the list of bulk generated tickets
                if (!ticketDtos.Any())
                    return NotFound(new { message = "No tickets generated" });
                return Ok(new ApiResponse<IEnumerable<TicketDto>>(200, ticketDtos, "Bulk tickets generated successfully"));
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
                return StatusCode(500, new { message = "An error occurred while bulk generating tickets", error = ex.Message });
            }
        }


        /// <summary>
        /// Add ticket to Google Wallet
        /// </summary>
        [HttpPost("add-to-google-wallet")]
        public async Task<ActionResult<WalletPassResponseDto>> AddToGoogleWallet([FromBody] AddToGoogleWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var walletUrl = await _ticketService.AddToGoogleWalletAsync(
                    request.TicketNumber,
                    request.EventName,
                    request.EventLocation,
                    request.EventDate);

                return Ok(new WalletPassResponseDto
                {
                    Success = true,
                    Message = "Google Wallet pass created successfully",
                    PassUrl = walletUrl,
                    WalletType = "Google Wallet"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new WalletPassResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    WalletType = "Google Wallet"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new WalletPassResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    WalletType = "Google Wallet"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new WalletPassResponseDto
                {
                    Success = false,
                    Message = "An error occurred while creating Google Wallet pass",
                    WalletType = "Google Wallet"
                });
            }
        }

        /// <summary>
        /// Add ticket to Apple Wallet
        /// </summary>
        [HttpPost("add-to-apple-wallet")]
        public async Task<ActionResult<WalletPassResponseDto>> AddToAppleWallet([FromBody] AddToAppleWalletDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var downloadUrl = await _ticketService.AddToAppleWalletAsync(
                    request.TicketNumber,
                    request.EventName,
                    request.EventLocation,
                    request.EventDate);

                return Ok(new WalletPassResponseDto
                {
                    Success = true,
                    Message = "Apple Wallet pass created successfully",
                    DownloadUrl = downloadUrl,
                    WalletType = "Apple Wallet"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new WalletPassResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    WalletType = "Apple Wallet"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new WalletPassResponseDto
                {
                    Success = false,
                    Message = ex.Message,
                    WalletType = "Apple Wallet"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new WalletPassResponseDto
                {
                    Success = false,
                    Message = "An error occurred while creating Apple Wallet pass",
                    WalletType = "Apple Wallet"
                });
            }
        }

        /// <summary>
        /// Download Apple Wallet pass file
        /// </summary>
        [HttpGet("{ticketNumber}/apple-wallet-pass")]
        public async Task<ActionResult> DownloadAppleWalletPass(string ticketNumber)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByNumberAsync(ticketNumber);
                if (ticket == null)
                    return NotFound(new { message = "Ticket not found" });

                // In a real implementation, you would retrieve the .pkpass file
                // For now, return a placeholder response
                return Ok(new { message = "Apple Wallet pass download endpoint", ticketNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while downloading the pass", error = ex.Message });
            }
        }



        private static TicketDto MapToTicketDto(Ticket ticket)
        {
            return new TicketDto
            {

                OrderId = ticket.OrderItemId??0,
                UserName = ticket.UserName ??"",
                OrderNumber = ticket.UserNumber??"",

                
                MemberShipId = ticket.MemberShipId??0,
                MemberName = ticket.MemberName??"",
                MembershipNumber = ticket.MembershipNumber ?? "",
                ExpiryDate = ticket.ExpiryDate,
                PurchaseDate = ticket.PurchaseDate,



                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                QRCode = ticket.QRCode,
                DeliveryMethod = ticket.DeliveryMethod,
                SentAt = ticket.SentAt,
                IsValid = ticket.ExpiryDate > DateTime.UtcNow && ticket.OrderItem?.Order.Status == OrderStatus.Paid,
               
                UserEmail = ticket.OrderItem?.Order.User?.Email ?? string.Empty,
                UserPhone = ticket.OrderItem?.Order.User?.Phone ?? string.Empty,

                PartnerEmail = ticket.MemberShip?.PartnerEmail ??null,
                PartnerName = ticket.MemberShip?.PartnerName ?? null,
                PartnerPhone = ticket.MemberShip?.PartnerPhone ?? null,
            };
        }
    }
}
