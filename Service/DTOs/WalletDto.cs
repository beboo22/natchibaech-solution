using System.ComponentModel.DataAnnotations;

namespace Travelsite.DTOs
{
    public class AddToGoogleWalletDto
    {
        [Required]
        public string TicketNumber { get; set; } = string.Empty;
        
        public string? EventName { get; set; }
        public string? EventLocation { get; set; }
        public DateTime? EventDate { get; set; }
    }

    public class AddToAppleWalletDto
    {
        [Required]
        public string TicketNumber { get; set; } = string.Empty;
        
        public string? EventName { get; set; }
        public string? EventLocation { get; set; }
        public DateTime? EventDate { get; set; }
    }

    public class WalletPassResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? PassUrl { get; set; }
        public string? DownloadUrl { get; set; }
        public string WalletType { get; set; } = string.Empty;
    }
}
