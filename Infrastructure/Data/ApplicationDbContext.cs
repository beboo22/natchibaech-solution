using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<MembershipCard> MembershipCards { get; set; }
        public DbSet<MemberShip> MemberShip { get; set; }
        public DbSet<MembershipReviewRequest> MembershipReviewRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasOne(u => u.Membership)
                      .WithOne(m => m.User)
                      .HasForeignKey<MemberShip>(m => m.UserId);
            });

            modelBuilder.Entity<MemberShip>().Property(m => m.QrCode).HasColumnType("NVARCHAR(MAX)");
            modelBuilder.Entity<MembershipCard>().Property(m => m.Price).HasColumnType("decimal(18,2)");

            // Order configurations
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId);
            });

            // OrderItem configurations
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId);
                      
                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId);
            });

            // DiscountCode configurations
            modelBuilder.Entity<DiscountCode>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasOne(dc => dc.User)
                      .WithMany(mc => mc.DiscountCodes)
                      .HasForeignKey(dc => dc.UserId);
                entity.HasOne(d=>d.MemberShip)
                .WithMany(m=>m.DiscountCodes)
                .HasForeignKey(d=>d.MemberShipId).OnDelete(DeleteBehavior.NoAction);//while deleting
            });

            // Transaction configurations
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.Order)
                      .WithMany(o => o.Transactions)
                      .HasForeignKey(t => t.OrderId);
                entity.HasOne(m=>m.MemberShip)
                .WithOne(m=>m.Transaction)
                .HasForeignKey<Transaction>(m=>m.MemberShipId);
            });

            // Ticket configurations
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(t=>t.OrderItemId).IsRequired(false);
                entity.Property(t=>t.MemberShipId).IsRequired(false);
                entity.Property(t => t.QRCode).HasColumnType("NVARCHAR(MAX)");
                entity.HasIndex(e => e.TicketNumber).IsUnique();
                entity.HasOne(t => t.OrderItem)
                      .WithOne(t=>t.Tickets)
                      .HasForeignKey<Ticket>(t => t.OrderItemId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(t => t.MemberShip)
                      .WithOne()
                      .HasForeignKey<Ticket>(t => t.MemberShipId).OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
