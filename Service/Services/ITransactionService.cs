using Domain.Entity;
using Travelsite.DTOs;

namespace TicketingSystem.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto> InitiateTransactionAsync(int orderId, decimal amount);
        Task<TransactionDto> MemberShipInitiateTransactionAsync(int MemberShipId, decimal amount);
        Task<Transaction?> ConfirmMemberShipTransactionAsync(string transactionReference, TransactionStatus status);
        Task<Transaction?> ConfirmOrderTransactionAsync(string transactionReference, TransactionStatus status);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByOrderIdAsync(int orderId);
        Task<Transaction?> GetTransactionByReferenceAsync(string transactionReference);
        Task<IEnumerable<Transaction>> GetTransactionsByMemberShipIdAsync(int MembershipCardId);
    }
}
