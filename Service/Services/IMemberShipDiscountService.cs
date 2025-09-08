using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IMemberShipDiscountService
    {
        Task<IEnumerable<MemberShipDiscountCode>> GetAllDiscountCodesAsync();
        Task<MemberShipDiscountCode?> GetDiscountCodeByCodeAsync(string code);
        Task<MemberShipDiscountCode> CreateDiscountCodeAsync(MemberShipDiscountCode discountCode);
        Task<decimal> ApplyMembershipDiscountAsync(string code, int? MemberShipId);
        Task<bool> ValidateDiscountCodeAsync(string code);
        Task<MemberShipDiscountCode?> UpdateDiscountCodeAsync(int id, MemberShipDiscountCode updatedDiscountCode);
        Task<bool> DeleteDiscountCodeAsync(int id);
    }
}
