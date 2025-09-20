using Domain.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace TicketingSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrCodeService;
        private readonly INotificationService _notificationService;
        private readonly IGoogleWalletService3 _googleWalletService;
        private readonly IAppleWalletService _appleWalletService;
        IConfiguration configuration;

        public TicketService(ApplicationDbContext context, IQRCodeService qrCodeService, INotificationService notificationService, IGoogleWalletService3 googleWalletService, IAppleWalletService appleWalletService, IConfiguration configuration)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _notificationService = notificationService;
            _googleWalletService = googleWalletService;
            _appleWalletService = appleWalletService;
            this.configuration = configuration;
        }


        // Added Google Wallet integration method
        public async Task<string> AddToGoogleWalletAsync(string ticketNumber, string? eventName = null, string? eventLocation = null, DateTime? eventDate = null)
        {
            var ticket = await GetTicketByNumberAsync(ticketNumber);
            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            //if ((ticket.OrderItem != null ? ticket.OrderItem.Order.Status : ticket.MemberShip.Status) != OrderStatus.Paid)
            //    throw new InvalidOperationException("Ticket must be from a paid order");

            if (ticket.OrderItem != null)
                if (ticket.OrderItem.Order.Status != OrderStatus.Paid)
                    throw new ArgumentException("Ticket must be from a paid order");
            if (ticket.MemberShip is not null)
                if (ticket.MemberShip.Status != OrderStatus.Paid)
                    throw new ArgumentException("Ticket must be from a paid Memper");

            if (ticket.ExpiryDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Ticket has expired");

            // Use provided event details or defaults
            var finalEventName = eventName ?? "Event";
            var finalEventLocation = eventLocation ?? "Event Location";
            var finalEventDate = eventDate ?? ticket.PurchaseDate;

            //// Create event class if needed (in production, you'd cache this)
            //await _googleWalletService.CreateEventTicketClassAsync(finalEventName, finalEventLocation, finalEventDate);

            //// Create the wallet pass URL
            //var walletUrl =   _googleWalletService.CreateEventTicketPassAsync(ticket);

            //var google = new GoogleWalletService3(configuration);
            var walletUrl = _googleWalletService.CreateTicketWalletLink(ticket);
            return walletUrl;
        }

        // Added Apple Wallet integration method
        public async Task<string> AddToAppleWalletAsync(string ticketNumber, string? eventName = null, string? eventLocation = null, DateTime? eventDate = null)
        {
            var ticket = await GetTicketByNumberAsync(ticketNumber);
            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            if (ticket.OrderItem.Order.Status != OrderStatus.Paid)
                throw new InvalidOperationException("Ticket must be from a paid order");

            if (ticket.ExpiryDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Ticket has expired");

            // Generate the Apple Wallet pass
            var passData = await _appleWalletService.CreateEventTicketPassAsync(ticket);

            // In a real implementation, you'd save this to a file server or blob storage
            // and return the download URL
            var downloadUrl = await _appleWalletService.GetPassDownloadUrlAsync(ticketNumber);
            return downloadUrl;
        }


        public async Task<IEnumerable<Ticket>> GenerateTicketsAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.Status != OrderStatus.Paid)
                throw new InvalidOperationException("Order must be paid before generating tickets");

            // Check if tickets already exist for this order
            var orderItemIds = order.OrderItems.Select(oi => oi.Id).ToList();

            var existingTickets = await _context.Tickets
                .Where(t => orderItemIds.Contains(t.OrderItemId.Value))
                .ToListAsync();

            if (existingTickets.Any())
                throw new InvalidOperationException("Tickets have already been generated for this order");

            //var existingTickets = await _context.Tickets
            //    .Where(t => t.OrderItem.Order.Id == orderId)//should check orderitem id??
            //    .ToListAsync();

            //if (existingTickets.Any())
            //    throw new InvalidOperationException("Tickets have already been generated for this order");

            var tickets = new List<Ticket>();

            // Generate tickets based on order items
            foreach (var orderItem in order.OrderItems)
            {

                var ticketNumber = GenerateTicketNumber();
                var UserName = $"{order.BillingFirstName} {order.BillingLastName}";
                var UserNumber = GenerateOrderNumber(order.UserId);

                var qrCode = _qrCodeService.GenerateTicketQRCode(ticketNumber, orderId, UserName);

                var ticket = new Ticket
                {
                    OrderItemId = orderItem.Id,
                    TicketNumber = ticketNumber,
                    UserName = UserName,
                    UserNumber = UserNumber,
                    PurchaseDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30), // Default 30 days validity
                    QRCode = qrCode,
                    DeliveryMethod = TicketDelivery.Email
                };

                tickets.Add(ticket);

            }
            try
            {

                _context.Tickets.AddRange(tickets);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tickets;
        }

        public async Task<Ticket?> GetTicketByNumberAsync(string ticketNumber)
        {
            return await _context.Tickets
                .Include(t => t.OrderItem)
                    .ThenInclude(o => o.Order)
                    .ThenInclude(o => o.User)

                    .Include(t => t.MemberShip).ThenInclude(u => u.User)
                //.ThenInclude(o=>o.Product)
                //.ThenInclude(o=>o.p)
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(string Email)
        {
            //var item = await _context.Orders
            //   .Include(o=>o.OrderItems)
            //   .ThenInclude(o=>o.Tickets)
            //   .Where(o => o.UserId == userId).Select(o=>o.OrderItems.ti).ToListAsync();


            //var item = await _context.OrderItems
            //    .Include(oi => oi.Order)
            //        .ThenInclude(o => o.User)
            //    .Include(oi => oi.Tickets)
            //    .Where(oi => oi.Order.UserId == userId)
            //    .Select(oi => oi.Tickets)
            //    .ToListAsync();


            //return await _context.Tickets
            //    .Include(t => t.Order)
            //        .ThenInclude(o => o.Order)
            //        .ThenInclude(Order => Order.User)
            //    //.Include(oi => oi.)
            //    .Where(t => t.Order.Order.UserId == userId)
            //    //.OrderByDescending(t => t.PurchaseDate)
            //    .ToListAsync();            //var item = await _context.OrderItems
            //    .Include(oi => oi.Order)
            //        .ThenInclude(o => o.User)
            //    .Include(oi => oi.Tickets)
            //    .Where(oi => oi.Order.UserId == userId)
            //    .Select(oi => oi.Tickets)
            //    .ToListAsync();


            //return await _context.Tickets
            //    .Include(t => t.Order)
            //        .ThenInclude(o => o.Order)
            //        .ThenInclude(Order => Order.User)
            //    //.Include(oi => oi.)
            //    .Where(t => t.Order.Order.UserId == userId)
            //    //.OrderByDescending(t => t.PurchaseDate)
            //    .ToListAsync();
            return await _context.Tickets
                .Where(t => t.OrderItem.Order.UserEmail == Email)
                .ToListAsync();

        }
       
        public async Task<IEnumerable<Ticket>> GetUserMembershipTicketsAsync(string Email)
        {
            //var item = await _context.Orders
            //   .Include(o=>o.OrderItems)
            //   .ThenInclude(o=>o.Tickets)
            //   .Where(o => o.UserId == userId).Select(o=>o.OrderItems.ti).ToListAsync();


            //var item = await _context.OrderItems
            //    .Include(oi => oi.Order)
            //        .ThenInclude(o => o.User)
            //    .Include(oi => oi.Tickets)
            //    .Where(oi => oi.Order.UserId == userId)
            //    .Select(oi => oi.Tickets)
            //    .ToListAsync();


            //return await _context.Tickets
            //    .Include(t => t.Order)
            //        .ThenInclude(o => o.Order)
            //        .ThenInclude(Order => Order.User)
            //    //.Include(oi => oi.)
            //    .Where(t => t.Order.Order.UserId == userId)
            //    //.OrderByDescending(t => t.PurchaseDate)
            //    .ToListAsync();            //var item = await _context.OrderItems
            //    .Include(oi => oi.Order)
            //        .ThenInclude(o => o.User)
            //    .Include(oi => oi.Tickets)
            //    .Where(oi => oi.Order.UserId == userId)
            //    .Select(oi => oi.Tickets)
            //    .ToListAsync();


            //return await _context.Tickets
            //    .Include(t => t.Order)
            //        .ThenInclude(o => o.Order)
            //        .ThenInclude(Order => Order.User)
            //    //.Include(oi => oi.)
            //    .Where(t => t.Order.Order.UserId == userId)
            //    //.OrderByDescending(t => t.PurchaseDate)
            //    .ToListAsync();
            return await _context.Tickets
                .Where(t => t.MemberShip.UserEmail == Email)
                .ToListAsync();

        }
       
        public async Task<bool> SendTicketAsync(int ticketId, TicketDelivery deliveryMethod, string email)
        {
            var ticket = await _context.Tickets
                .Include(t => t.OrderItem)
                    .ThenInclude(o => o.Order)
                    .ThenInclude(o => o.User)
                .Include(u => u.MemberShip).ThenInclude(u => u.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return false;

            var user = ticket.OrderItem?.Order.User ?? ticket.MemberShip?.User;
            if (user == null) return false;

            var isCouple = _context.MemberShips.Include(m=>m.MembershipCard)
                .FirstOrDefault(t => t.Id == ticket.MemberShipId)?
                .MembershipCard.ServiceCategory == ServiceCategory.Couples;

            bool success = false;

            try
            {
                switch (deliveryMethod)
                {
                    case TicketDelivery.Email:
                        success = await SendEmailAsync(ticket, user.Phone, email, isCouple);
                        break;

                    case TicketDelivery.Whatsapp:
                        //success = await SendWhatsAppAsync(ticket, user.Phone);
                        break;

                    case TicketDelivery.Both:
                        var emailSuccess = await SendEmailAsync(ticket, user.Phone, email, isCouple);
                        //var whatsappSuccess = await SendWhatsAppAsync(ticket, user.Phone);
                        success = emailSuccess;
                        break;

                    case TicketDelivery.None:
                        success = true; // No delivery required
                        break;
                }

                if (success)
                {
                    ticket.DeliveryMethod = deliveryMethod;
                    ticket.SentAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return success;
            }
            catch (Exception ex)
            {
                // TODO: log ex (Serilog/NLog/etc.)
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(Ticket ticket, string phone, string? email, bool isCouple)
        {
            if (isCouple)
            {
                return await _notificationService.SendCoupleTicketByEmailAsync(
                    email,
                    ticket.TicketNumber,
                    ticket.QRCode,
                    ticket.MemberShip is not null ? ticket.MemberName : ticket.UserName,
                    ticket.MemberShip?.BookingDate ?? ticket.OrderItem.BookingDate,
                    ticket.MemberShip.Expiry,
                    phone,
                    ticket.MemberShip?.PartnerName,
                    ticket.MemberShip?.PartnerEmail,
                    ticket.MemberShip?.PartnerPhone,
                    ticket.MemberShip?.PartnerSsn,
                    ticket.MemberShip.User.Ssn);
            }
            else
            {
                return await _notificationService.SendSingleTicketByEmailAsync(
                    email,
                    ticket.TicketNumber,
                    ticket.QRCode,
                    ticket.MemberShip is not null ? ticket.MemberName : ticket.UserName,
                    ticket.MemberShip?.BookingDate ?? ticket.OrderItem.BookingDate,
                    ticket.MemberShip.Expiry,
                    phone,
                    ticket.MemberShip.User.Ssn);
            }
        }

        //private async Task<bool> SendWhatsAppAsync(Ticket ticket, string phone)
        //{
        //    return await _notificationService.SendTicketByWhatsAppAsync(
        //        phone,
        //        ticket.TicketNumber,
        //        ticket.QRCode,
        //        ticket.MemberName,
        //        ticket.ExpiryDate);
        //}

        public async Task<bool> ValidateTicketAsync(string ticketNumber)
        {
            var ticket = await GetTicketByNumberAsync(ticketNumber);

            if (ticket == null)
                return false;

            // Check if ticket is expired
            if (ticket.ExpiryDate <= DateTime.UtcNow)
                return false;

            // Check if order is paid
            if (ticket.OrderItem.Order.Status != OrderStatus.Paid)
                return false;

            return true;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByOrderAsync(int orderId)
        {
            //var item = await _context.OrderItems
            //   .Include(oi => oi.Order)
            //   .ThenInclude(o => o.User)
            //   .Include(oi => oi.Product)
            //   .Include(oi => oi.Tickets)
            //   .Where(oi => oi.OrderId == orderId)
            //   .Select(oi => oi.Tickets)
            //   .ToListAsync();


            var item = await _context.Tickets
                .Include(t => t.OrderItem)
                    .ThenInclude(oi => oi.Order)
                        .ThenInclude(o => o.User)
                .Include(t => t.OrderItem)
                .Where(t => t.OrderItem.OrderId == orderId)
                .ToListAsync();
            return item;
        }

        private static string GenerateTicketNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"TKT{timestamp}{random}";
        }

        private static string GenerateMembershipNumber(int userId)
        {
            return $"MEM{userId:D6}{DateTime.UtcNow.Year}";
        }
        private static string GenerateOrderNumber(int userId)
        {
            return $"ORD{userId:D6}{DateTime.UtcNow.Year}";
        }

        public async Task<Ticket> GenerateMemberTicketsAsync(int MemberId)
        {
            var order = await _context.MemberShips
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == MemberId);

            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.Status != OrderStatus.Paid)
                throw new InvalidOperationException("Order must be paid before generating tickets");

            // Check if tickets already exist for this order
            var existingTickets = await _context.Tickets
                .Where(t => t.MemberShipId == MemberId)
                .ToListAsync();

            if (existingTickets.Any())
                throw new InvalidOperationException("Tickets have already been generated for this MemberShip");
            var ticketNumber = GenerateTicketNumber();
            var memberName = $"{order.User.FullName}";
            var membershipNumber = GenerateMembershipNumber(order.UserId);

            var qrCode = _qrCodeService.GenerateTicketQRCode(ticketNumber, MemberId, memberName);

            var item = new Ticket
            {
                MemberShipId = MemberId,
                TicketNumber = ticketNumber,
                MemberName = memberName,
                MembershipNumber = membershipNumber,
                PurchaseDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30), // Default 30 days validity
                QRCode = qrCode,
                DeliveryMethod = TicketDelivery.Email,
                //UserName
            };
            await _context.Tickets.AddAsync(item);

            await _context.SaveChangesAsync();


            return item;


        }
    }
}
