using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class MembershipCard
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<MemberShip> MemberShips { get; set; } = new List<MemberShip>();

    }

    public class MemberShip
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MembershipCardId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public bool IsActive { get; set; } = true;
        public string MembershipNumber { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public DateTime Expiry { get; set; }
        public string QrCode { get; set; } = string.Empty;


        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual MembershipCard MembershipCard { get; set; } = null!;
        public virtual Transaction Transaction { get; set; } = null!;
        public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
        //public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();



    }


}
