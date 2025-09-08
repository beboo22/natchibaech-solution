using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{

    public class MemberShipDiscountCode
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        
        public int Percentage { get; set; }
        
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; } = 0;
        public bool IsActive { get; set; }


        public DateTime ExpiryDate { get; set; }
        public virtual ICollection<MemberShip>? MemberShip { get; set; } = new List<MemberShip>();
    }
    public class OrderDiscountCode
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        
        public int Percentage { get; set; }
        
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; } = 0;
        public bool IsActive { get; set; }


        public DateTime ExpiryDate { get; set; }
        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();
    }


}
