using Domain.Entity;

namespace TicketingSystem.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GenerateTicketsAsync(int orderId);
        Task<Ticket> GenerateMemberTicketsAsync(int MemberId);
        Task<Ticket?> GetTicketByNumberAsync(string ticketNumber);
        Task<IEnumerable<Ticket>> GetUserTicketsAsync(string Email);
        Task<IEnumerable<Ticket>> GetUserMembershipTicketsAsync(string Email);
        Task<bool> SendTicketAsync(int ticketId, TicketDelivery deliveryMethod,string Email);
        Task<bool> ValidateTicketAsync(string ticketNumber);
        Task<IEnumerable<Ticket>> GetTicketsByOrderAsync(int orderId);
        Task<string> AddToGoogleWalletAsync(string ticketNumber, string? eventName = null, string? eventLocation = null, DateTime? eventDate = null);
        Task<string> AddToAppleWalletAsync(string ticketNumber, string? eventName = null, string? eventLocation = null, DateTime? eventDate = null);
    }
}
