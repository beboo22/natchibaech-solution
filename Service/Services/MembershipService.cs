using Microsoft.EntityFrameworkCore;
using Domain.Entity;
using Infrastructure.Data;
using Travelsite.DTOs;

namespace TicketingSystem.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext _context;
        private readonly IQRCodeService _qrCodeService;
        private readonly INotificationService _notificationService;

        public MembershipService(ApplicationDbContext context, IQRCodeService qrCodeService, INotificationService notificationService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
            _notificationService = notificationService;
        }

        public async Task<MemberShip> IssueMembershipAsync(IssueMembershipCardDto issue)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == issue.userEmail);

            var card = await _context.MembershipCards
                .FirstOrDefaultAsync(c => c.Id == issue.MembershipCardId);
            if(await _context.MemberShips.AnyAsync(u=>u.User.Email == issue.userEmail))
                throw new Exception("There's Membership Already exist");





            if (card == null)
                throw new InvalidOperationException("No MembershipCard found with the provided Id.");

            if (card.ServiceCategory == ServiceCategory.Couples &&
                (string.IsNullOrEmpty(issue.PartnerEmail) ||
                string.IsNullOrEmpty(issue.PartnerEmail) ||
                string.IsNullOrEmpty(issue.PartnerPhone)))
                throw new InvalidOperationException("Should add Partner data");

            if (user == null)
            {
                user = new User
                {
                    FullName = issue.Fullname,
                    Email = issue.userEmail,
                    Phone = issue.phone,
                    Ssn = issue.Ssn,
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            // Handle user-category vs card-category restrictions using switch
            if (card.ServiceCategory == ServiceCategory.Men)
            {
                //need to ckeack existance

               //var Existuser = await _context.MembershipReviewRequests.FirstOrDefaultAsync(m => m.UserEmail == user.Email&& m.Status == ReviewStatus.Approved);
               // if (Existuser is not null)
               // {
               //     throw new Exception("There's Membership Already exist");
               // }


                var reviewRequest = new MembershipReviewRequest
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    MembershipCardId = issue.MembershipCardId,
                    RequestedAt = DateTime.UtcNow,
                    Status = ReviewStatus.Pending
                };
                _context.MembershipReviewRequests.Add(reviewRequest);
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("Thank you for registering for Natchi Beach Membership \nYour request is under review, and we will get back to you soon. We’re happy to have you with us!");
            }

            // Ensure user doesn't already have this membership card
            var existingCard = await _context.MembershipCards
                .FirstOrDefaultAsync(mc => mc.Id == issue.MembershipCardId);

            if (existingCard == null)
                throw new InvalidOperationException("Membership card not found.");

            var membershipNumber = GenerateMembershipNumber(user.Id);
            var qrCodeData = GenerateMembershipQRData(
                membershipNumber,
                user.FullName,
                user.Ssn,
                DateTime.UtcNow.AddYears(1)
            );

            var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);


            var membership = new MemberShip
            {
                MembershipCardId = issue.MembershipCardId,
                UserId = user.Id,
                UserEmail = issue.userEmail,
                MembershipNumber = membershipNumber,
                BookingDate = DateTime.UtcNow,
                Expiry = DateTime.UtcNow.AddYears(1),
                QrCode = qrCode,
                Status = OrderStatus.Pending,
            };

            if (card.ServiceCategory == ServiceCategory.Couples)
            {
                membership.PartnerPhone = issue.PartnerPhone;
                membership.PartnerName = issue.PartnerName;
                membership.PartnerEmail = issue.PartnerEmail;
                membership.PartnerSsn = issue.PartnerSsn;
            }
            _context.MemberShips.Add(membership);
            await _context.SaveChangesAsync();

            membership.MembershipCard = existingCard;
            membership.User = user;

            // Send email
            //await SendMembershipCardAsync(membership);

            return membership;
        }


        //method to complete the men memberShip after review
        public async Task<MemberShip> CompleteMembershipIssuanceAsync(int reviewRequestId)
        {
            // Verify review request
            var reviewRequest = await _context.MembershipReviewRequests
                .FirstOrDefaultAsync(rr => rr.Id == reviewRequestId);

            if (reviewRequest == null)
                throw new ArgumentException("Review request not found");

            if (reviewRequest.Status == ReviewStatus.Rejected)
                throw new InvalidOperationException("Membership card issuance cannot proceed because the review is not approved");

            // Retrieve user
            var user = await _context.Users.FindAsync(reviewRequest.UserId);
            if (user == null)
                throw new ArgumentException("User not found");



            // Generate membership number and QR code
            var membershipNumber = GenerateMembershipNumber(reviewRequest.UserId);
            var isDuplicate = await _context.MemberShips.AnyAsync(mc => mc.MembershipNumber == membershipNumber);
            if (isDuplicate)
                throw new InvalidOperationException("Generated membership number already exists.");

            string qrCode;
            try
            {
                var qrCodeData = GenerateMembershipQRData(membershipNumber, user.FullName, user.Ssn, DateTime.UtcNow.AddYears(1));
                qrCode = _qrCodeService.GenerateQRCode(qrCodeData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate QR code.", ex);
            }

            // Create membership card
            var membershipCard = new MemberShip
            {
                MembershipCardId = reviewRequest.MembershipCardId,
                UserId = reviewRequest.UserId,
                UserEmail = reviewRequest.UserEmail,
                MembershipNumber = membershipNumber,
                BookingDate = DateTime.UtcNow,
                Expiry = DateTime.UtcNow.AddYears(1),
                QrCode = qrCode,
            };

            // Save membership card and send email within a transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.MemberShips.Add(membershipCard);
                    await _context.SaveChangesAsync();
                    await _notificationService.SendApprovedEmail(reviewRequest.UserEmail,user.FullName);


                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return membershipCard;
        }



        public async Task<MemberShip?> GetMembershipAsync(string Email)
        {
            return await _context.MemberShips
                .Include(mc => mc.User)
                .Include(m => m.MembershipCard)
                .FirstOrDefaultAsync(mc => mc.User.Email == Email);
        }

        public async Task<IEnumerable<MemberShip>> GetAllMembershipAsync()
        {
            return await _context.MemberShips
                .Include(mc => mc.User)
                .Include(mc => mc.MemberShipDiscountCode)
                .OrderByDescending(mc => mc.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MemberShip>> GetActiveMembershipAsync()
        {
            return await _context.MemberShips
                .Include(mc => mc.User)
                .Include(mc => mc.MemberShipDiscountCode)
                .Where(mc => mc.Expiry > DateTime.UtcNow)
                .OrderByDescending(mc => mc.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MemberShip>> GetExpiringMembershipAsync(int daysFromNow = 30)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysFromNow);

            return await _context.MemberShips
                .Include(mc => mc.User)
                .Where(mc => mc.Expiry <= expiryDate && mc.Expiry > DateTime.UtcNow)
                .OrderBy(mc => mc.Expiry)
                .ToListAsync();
        }

        public async Task<MemberShip?> UpdateMembershipStatusAsync(int membershipId, OrderStatus item)
        {
            var membershipCard = await _context.MemberShips.Include(u => u.User)
                .FirstOrDefaultAsync(mc => mc.Id == membershipId);

            if (membershipCard == null)
                return null;



            membershipCard.Status = item;
            var qrCodeData = GenerateMembershipQRData(membershipCard.MembershipNumber, membershipCard.User.FullName, membershipCard.User.Ssn, membershipCard.Expiry);
            var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);
            membershipCard.QrCode = qrCode;
            membershipCard.IsActive = true;
            try
            {
                await _context.SaveChangesAsync();
                //await SendMembershipCardAsync(membershipCard);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return membershipCard;
        }

        public async Task<MemberShip?> UpdateMembershipAsync(string Email, UpdateMembershipDto item)
        {
            var membershipCard = await _context.MemberShips.Include(u => u.User).Include(u => u.MembershipCard)
                .FirstOrDefaultAsync(mc => mc.User.Email == Email);

            if (membershipCard == null)
                return null;


            membershipCard.Expiry = item.Expiry;
            membershipCard.IsActive = item.IsActive.HasValue ? item.IsActive.Value : membershipCard.IsActive;
            membershipCard.Status = item.Status;
            var qrCodeData = GenerateMembershipQRData(membershipCard.MembershipNumber, membershipCard.User.FullName, membershipCard.User.Ssn, membershipCard.Expiry);
            var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);
            membershipCard.QrCode = qrCode;
            //membershipCard.pr
            try
            {
                await _context.SaveChangesAsync();
                //await SendMembershipCardAsync(membershipCard);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return membershipCard;
        }

        public async Task<bool> RenewMembershipAsync(string Email, int months = 12)
        {
            var membershipCard = await _context.MemberShips.Include(u => u.User).Include(u => u.MembershipCard)
                .FirstOrDefaultAsync(mc => mc.User.Email == Email);

            if (membershipCard == null)
                return false;
            //get user
            if (membershipCard.MembershipCard.ServiceCategory == ServiceCategory.Men)
            {
                // should admin review this?
                var reviewRequest = new MembershipReviewRequest
                {
                    UserId = membershipCard.User.Id,
                    RequestedAt = DateTime.UtcNow,
                    Status = ReviewStatus.Pending
                };
                membershipCard.Status = OrderStatus.Pending;
                _context.MembershipReviewRequests.Add(reviewRequest);
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("Membership card issuance for this category requires admin review.");
            }

            // Extend expiry date
            var newExpiry = membershipCard.Expiry > DateTime.UtcNow
                ? membershipCard.Expiry.AddMonths(months)
                : DateTime.UtcNow.AddMonths(months);
            membershipCard.IsActive = true;
            membershipCard.Expiry = newExpiry;
            membershipCard.IsActive = false;
            membershipCard.Status = OrderStatus.Pending;
            membershipCard.MembershipNumber = GenerateMembershipNumber(membershipCard.User.Id);
            var qrCodeData = GenerateMembershipQRData(membershipCard.MembershipNumber, membershipCard.User.FullName, membershipCard.User.Ssn, membershipCard.Expiry);
            var qrCode = _qrCodeService.GenerateQRCode(qrCodeData);
            membershipCard.QrCode = qrCode;

            await _context.SaveChangesAsync();

            // Send renewal notification
            //await SendMembershipCardAsync(membershipCard);

            return true;
        }

        public async Task<bool> ValidateMembershipAsync(string membershipNumber)
        {
            var membershipCard = await _context.MemberShips
                .FirstOrDefaultAsync(mc => GenerateMembershipNumber(mc.UserId) == membershipNumber);

            if (membershipCard == null)
                return false;

            return membershipCard.Expiry > DateTime.UtcNow;
        }

        public async Task<bool> RevokeMembershipAsync(string Email)
        {
            var membershipCard = await _context.MemberShips.Include(m=>m.Ticket)
                .FirstOrDefaultAsync(mc => mc.User.Email == Email);

            if (membershipCard == null)
                return false;

            _context.MemberShips.Remove(membershipCard);
            try
            {
            await _context.SaveChangesAsync();

            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }
        public async Task<MembershipStatsDto> GetMembershipStatsAsync()
        {
            var now = DateTime.UtcNow;
            var nextMonth = now.AddDays(30);

            // Query directly from DB (more efficient)
            var total = await _context.MemberShips.CountAsync();
            var active = await _context.MemberShips.CountAsync(mc => mc.Expiry > now);
            var expired = await _context.MemberShips.CountAsync(mc => mc.Expiry <= now);
            var expiringSoonCount = await _context.MemberShips.CountAsync(mc => mc.Expiry > now && mc.Expiry <= nextMonth);

            var membersByCategory = await _context.MemberShips
                .Where(mc => mc.Expiry > now)
                .GroupBy(mc => mc.MembershipCard.ServiceCategory)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key, g => g.Count);

            var recentlyIssued = await _context.MemberShips
                .OrderByDescending(mc => mc.BookingDate)
                .Include(mc => mc.User)
                .Take(5)
                .ToListAsync();

            var expiringSoon = await _context.MemberShips
                .Where(mc => mc.Expiry > now && mc.Expiry <= nextMonth)
                .OrderBy(mc => mc.Expiry)
                .Include(mc => mc.User)
                .Take(10)
                .ToListAsync();

            return new MembershipStatsDto
            {
                TotalMembers = total,
                ActiveMembers = active,
                ExpiredMembers = expired,
                ExpiringThisMonth = expiringSoonCount,
                MembersByCategory = membersByCategory,
                RecentlyIssued = recentlyIssued,
                ExpiringSoon = expiringSoon,
                ActivePercentage = total == 0 ? 0 : Math.Round((double)active / total * 100, 2),
                ExpiredPercentage = total == 0 ? 0 : Math.Round((double)expired / total * 100, 2)
            };
        }
        //private async Task SendMembershipCardAsync(MemberShip membershipCard)
        //{
        //    try
        //    {
        //        var user = await _context.Users.FindAsync(membershipCard.UserId);
        //        if (user == null) return;

        //        var membershipNumber = GenerateMembershipNumber(membershipCard.UserId);

        //        // This would integrate with your email service
        //        // For now, we'll just log it
        //        if(membershipCard.MembershipCard.ServiceCategory == ServiceCategory.Couples)
        //            await _notificationService.SendCoupleTicketByEmailAsync(user.Email, )

        //        await _notificationService.SendTicketByEmailAsync(
        //            user.Email, membershipNumber, membershipCard.QrCode, user.FullName, membershipCard.Expiry);
        //    }
        //    catch (Exception)
        //    {
        //        // Log error but don't fail the membership card creation
        //        throw;
        //    }
        //}



        private static string GenerateMembershipNumber(int userId)
        {
            return $"MEM{userId:D6}{DateTime.UtcNow.Year}";
        }

        private static string GenerateMembershipQRData(string membershipNumber, string memberName, string category, DateTime time)
        {
            return $"MEMBERSHIP:{membershipNumber}|NAME:{memberName}|CATEGORY:{category}|ISSUED:{DateTime.UtcNow:yyyy-MM-dd}|Expiry Data :{time:yyyy-MM-dd}";
        }


    }



}
