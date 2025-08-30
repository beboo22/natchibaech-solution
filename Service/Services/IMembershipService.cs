using Domain.Entity;
using Travelsite.DTOs;

namespace TicketingSystem.Services
{
    public interface IMembershipService
    {
        Task<MemberShip> CompleteMembershipIssuanceAsync(int reviewRequestId);
        Task<MemberShip> IssueMembershipAsync(IssueMembershipCardDto issue);
        Task<MemberShip?> GetMembershipAsync(string Email);
        Task<MemberShip?> UpdateMembershipStatusAsync(int membershipId, OrderStatus item);
        Task<IEnumerable<MemberShip>> GetAllMembershipAsync();
        Task<IEnumerable<MemberShip>> GetActiveMembershipAsync();
        Task<IEnumerable<MemberShip>> GetExpiringMembershipAsync(int daysFromNow = 30);
        Task<MemberShip?> UpdateMembershipAsync(string Email, UpdateMembershipDto item);
        Task<bool> RenewMembershipAsync(string Email, int months = 12);
        Task<bool> ValidateMembershipAsync(string membershipNumber);
        Task<bool> RevokeMembershipAsync(string Email);
        Task<MembershipStatsDto> GetMembershipStatsAsync();
    }
}
