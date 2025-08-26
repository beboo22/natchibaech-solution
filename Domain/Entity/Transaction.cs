using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class Transaction
    {
        public int Id { get; set; }
        
        public int? OrderId { get; set; }
        public int? MemberShipId { get; set; }



        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        
        [MaxLength(100)]
        public string TransactionReference { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Order? Order { get; set; } = null!;
        public MemberShip? MemberShip { get; set; }
    }
}
