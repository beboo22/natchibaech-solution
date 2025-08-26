using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Services;
using Travelsite.Middleware;

namespace Travelsite
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            

            builder.Services.AddHttpClient<IPaymobService, PaymobService>();


            builder.Services.AddScoped<IPaymobService, PaymobService>();



            builder.Services.AddHttpClient<IGoogleWalletService3, GoogleWalletService3>();
            builder.Services.AddScoped<IGoogleWalletService3, GoogleWalletService3>();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();
            builder.Services.AddScoped<IQRCodeService, QRCodeService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IGoogleWalletService, GoogleWalletService>();
            builder.Services.AddScoped<IAppleWalletService, AppleWalletService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            //var (certPath, certPassword) = await SslCertificateManager.ObtainAndConfigureSslCertificate(
            //domain: "travelsite.runasp.net",
            //email: "moammedtareq8@gmail.com",
            //outputPath: "Certs",
            //certPassword: "your-secure-password",
            //useStaging: false // Set to false for production
            //);
            //builder.ConfigureHttps(certPath, certPassword);

            var app = builder.Build();
            // add exception handling middleware
            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();            
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
