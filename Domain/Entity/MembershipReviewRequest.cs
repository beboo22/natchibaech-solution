using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class MembershipReviewRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;


        public int MembershipCardId { get; set; }
        public MembershipCard MembershipCard { get; set; } = null!;

        public DateTime RequestedAt { get; set; }
        public ReviewStatus Status { get; set; }
    }
}
