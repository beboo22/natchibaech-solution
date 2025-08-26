using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs
{
    public class MembershipReviewRequestDtos
    {
        public int UserId { get; set; }
        public DateTime RequestedAt { get; set; }
        public ReviewStatus Status { get; set; }
    }
}
