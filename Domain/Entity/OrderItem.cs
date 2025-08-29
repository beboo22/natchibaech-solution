using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }


        //public DateTime createdAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ServiceCategory ServiceCategory { get; set; }

        public DateTime BookingDate { get; set; }

        //public int ProductId { get; set; }


        public int PersonNumber { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        //public virtual Product Product { get; set; } = null!;
        public virtual Ticket Tickets { get; set; }

    }
}
