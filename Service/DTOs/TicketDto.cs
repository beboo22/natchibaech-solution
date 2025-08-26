using Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Travelsite.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? MemberShipId { get; set; }

        public string TicketNumber { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string MembershipNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string QRCode { get; set; } = string.Empty;
        public TicketDelivery DeliveryMethod { get; set; }
        public DateTime? SentAt { get; set; }
        public bool IsValid { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
    }

    public class GenerateTicketsDto
    {
        public string? Email { get; set; }
        public TicketDelivery DeliveryMethod { get; set; } = TicketDelivery.Email;
        
        public int ValidityDays { get; set; } = 30;
    }

    public class SendTicketDto
    {
        [Required]
        public TicketDelivery DeliveryMethod { get; set; }
        
        public string? CustomEmail { get; set; }
        public string? CustomPhone { get; set; }
        public string? CustomMessage { get; set; }
    }

    public class TicketValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public TicketDto? Ticket { get; set; }
        public string ValidationCode { get; set; } = string.Empty;
    }

    public class BulkTicketGenerationDto
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        [Range(1, 100, ErrorMessage = "Number of tickets must be between 1 and 100")]
        public int NumberOfTickets { get; set; }
        
        public TicketDelivery DeliveryMethod { get; set; } = TicketDelivery.Email;
        public int ValidityDays { get; set; } = 30;
        public List<string> MemberNames { get; set; } = new List<string>();
    }
}
