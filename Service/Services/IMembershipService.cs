using Domain.Entity;
using Travelsite.DTOs;

namespace TicketingSystem.Services
{
    public interface IMembershipService
    {
        Task<MemberShip> IssueMembershipAsync(IssueMembershipCardDto issue);
        Task<MemberShip?> GetMembershipAsync(int userId);
        Task<MemberShip?> UpdateMembershipStatusAsync(int userId, OrderStatus item);
        Task<IEnumerable<MemberShip>> GetAllMembershipAsync();
        Task<IEnumerable<MemberShip>> GetActiveMembershipAsync();
        Task<IEnumerable<MemberShip>> GetExpiringMembershipAsync(int daysFromNow = 30);
        Task<MemberShip?> UpdateMembershipAsync(int userId, UpdateMembershipDto item);
        Task<bool> RenewMembershipAsync(int userId, int months = 12);
        Task<bool> ValidateMembershipAsync(string membershipNumber);
        Task<bool> RevokeMembershipAsync(int userId);
        Task<MembershipStatsDto> GetMembershipStatsAsync();
    }
}
