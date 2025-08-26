using System.ComponentModel.DataAnnotations;

namespace Travelsite.DTOs
{
    public class DiscountCodeDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int RemainingUsage { get; set; }
    }

    public class CreateDiscountCodeDto
    {
        public int? UserId { get; set; }
        public int? MemberId { get; set; }
        
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Discount code must contain only uppercase letters and numbers")]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 100, ErrorMessage = "Percentage must be between 1 and 100")]
        public int Percentage { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Max usage must be at least 1")]
        public int MaxUsage { get; set; }
        
        [Required]
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class ApplyDiscountDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        public int OrderId { get; set; }
    }

    public class DiscountResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public int DiscountPercentage { get; set; }
        public string DiscountCode { get; set; } = string.Empty;
    }
}
