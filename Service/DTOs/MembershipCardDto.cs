using Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Travelsite.DTOs
{
    public class MembershipCardDto
    {
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class MembershipDto
    {
        //public int Id { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public OrderStatus status { get; set; }
        public UserType UserCategory { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime Expiry { get; set; }
        public string QrCode { get; set; }
        public int DaysUntilExpiry { get; set; }
        public string MembershipNumber { get; set; }
        public List<DiscountCodeDto> AvailableDiscounts { get; set; }
    }

    public class IssueMembershipCardDto
    {
        [Required]
        [EmailAddress]
        public string userEmail { get; set; }
        [Required]
        public int MembershipCardId { get; set; }
        
        
        public int ValidityMonths { get; set; } = 12; // Default 1 year
        public decimal Price { get; set; } = 7000.00m;
        
        //public OrderStatus Status { get; set; }

        //public UpdateMembershipCardDto UpdatemembershipCardDto { get; set; }
    }

    public class UpdateMembershipDto
    {        
        [Required]
        public DateTime Expiry { get; set; }
        public bool? IsActive { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class MembershipCardValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public MembershipCardDto? MembershipCard { get; set; }
        public string ValidationCode { get; set; } = string.Empty;
    }

  
    public class MembershipStatsDto
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int ExpiredMembers { get; set; }
        public int ExpiringThisMonth { get; set; }
        public Dictionary<UserType, int> MembersByCategory { get; set; } = new();
        public List<MemberShip> RecentlyIssued { get; set; } = new();
        public List<MemberShip> ExpiringSoon { get; set; } = new();
        public double ActivePercentage { get; set; }
        public double ExpiredPercentage { get; set; }
    }
}
