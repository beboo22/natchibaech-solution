using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class MembershipCard
    {
        public int Id { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<MemberShip> MemberShips { get; set; } = new List<MemberShip>();

    }

    public class MemberShip
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [EmailAddress]
        public string UserEmail { get; set; } = null!;
        public string? PartnerEmail { get; set; }
        public string? PartnerName { get; set; }
        public string? PartnerPhone { get; set; }
        public string? PartnerSsn { get; set; }

        public int MembershipCardId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public bool IsActive { get; set; } = true;
        public string MembershipNumber { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public DateTime Expiry { get; set; }
        public string QrCode { get; set; } = string.Empty;

        public virtual int? MemberShipDiscountCodeId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? ApplyDiscount { get; set; } = false;
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual MembershipCard MembershipCard { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = null!;
        public virtual MemberShipDiscountCode? MemberShipDiscountCode { get; set; }
    }


}
