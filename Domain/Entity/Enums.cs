namespace Domain.Entity
{
    public enum UserCategory
    {
        Ladies,
        Couples,
        Men
    }
    public enum ReviewStatus
    {
        Pending,
        Approved,
        Rejected,
    }

    public enum OrderStatus
    {
        Pending,
        Approved,
        Rejected,
        Paid
    }

    public enum TransactionStatus
    {
        Success,
        Failed,
        Pending
    }

    public enum PaymentMethod
    {
        Paymob,
        Wallet,
        Cash
    }

    public enum TicketDelivery
    {
        Email,
        Whatsapp,
        Both,
        None
    }
}
