namespace Domain.Entity
{
    public enum ServiceCategory
    {
        Ladies,
        Men,
        Couples
    }
    public enum UserType
    {
        male,
        female
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
        Pending,
        Success,
        Failed,
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
