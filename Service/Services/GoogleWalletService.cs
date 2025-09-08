using Domain.Entity;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace TicketingSystem.Services
{
    #region V01
    //public class GoogleWalletService : IGoogleWalletService
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly HttpClient _httpClient;

    //    public GoogleWalletService(IConfiguration configuration, HttpClient httpClient)
    //    {
    //        _configuration = configuration;
    //        _httpClient = httpClient;
    //    }

    //    public string CreateEventTicketPassAsync(Ticket ticket)
    //    {
    //        var passObject = new
    //        {
    //            id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.{ticket.TicketNumber}",
    //            classId = $"{_configuration["GoogleWalletSettings:IssuerId"]}.{_configuration["GoogleWalletSettings:ClassId"]}",
    //            state = "ACTIVE",
    //            eventTicketObject = new
    //            {
    //                id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.{ticket.TicketNumber}",
    //                classReference = new
    //                {
    //                    id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class"
    //                },
    //                state = "ACTIVE",
    //                ticketHolderName = ticket.MemberName,
    //                ticketNumber = ticket.TicketNumber,
    //                barcode = new
    //                {
    //                    type = "QR_CODE",
    //                    value = ticket.QRCode,
    //                    alternateText = ticket.TicketNumber
    //                },
    //                seatInfo = new
    //                {
    //                    seat = new
    //                    {
    //                        seatNumber = ticket.MembershipNumber
    //                    }
    //                },
    //                ticketType = new
    //                {
    //                    localizedValue = new
    //                    {
    //                        defaultValue = new
    //                        {
    //                            language = "en-US",
    //                            value = "General Admission"
    //                        }
    //                    }
    //                },
    //                validTimeInterval = new
    //                {
    //                    start = new
    //                    {
    //                        date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    //                    },
    //                    end = new
    //                    {
    //                        date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    //                    }
    //                }
    //            }
    //        };

    //        // Generate JWT token for Google Wallet
    //        var jwt = GenerateGoogleWalletJWT(passObject);
    //        return $"https://pay.google.com/gp/v/save/{jwt}";
    //    }

    //    public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate)
    //    {
    //        var classObject = new
    //        {
    //            id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class",
    //            issuerName = _configuration["GoogleWalletSettings:IssuerName"],
    //            eventName = new
    //            {
    //                defaultValue = new
    //                {
    //                    language = "en-US",
    //                    value = eventName
    //                }
    //            },
    //            venue = new
    //            {
    //                name = new
    //                {
    //                    defaultValue = new
    //                    {
    //                        language = "en-US",
    //                        value = eventLocation
    //                    }
    //                }
    //            },
    //            dateTime = new
    //            {
    //                start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    //            },
    //            reviewStatus = "UNDER_REVIEW"
    //        };

    //        var jwt = GenerateGoogleWalletJWT(classObject);
    //        return jwt;
    //    }

    //    private string GenerateGoogleWalletJWT(object payload)
    //    {
    //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];

    //        var credential = (ServiceAccountCredential)GoogleCredential
    //            .FromFile(serviceAccountKeyPath)
    //            .UnderlyingCredential;

    //        var header = new { alg = "RS256", typ = "JWT" };
    //        var claims = new
    //        {
    //            iss = credential.Id,
    //            aud = "google",
    //            typ = "savetowallet",
    //            payload = payload,
    //            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    //            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
    //        };

    //        string headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
    //        string claimsBase64 = Base64UrlEncode(JsonSerializer.Serialize(claims));
    //        string unsignedJwt = $"{headerBase64}.{claimsBase64}";

    //        // Fix: Use RSA from credential.Key
    //        using var rsa = credential.Key; // this is an RSA key
    //        var signature = rsa.SignData(
    //            System.Text.Encoding.UTF8.GetBytes(unsignedJwt),
    //            HashAlgorithmName.SHA256,
    //            RSASignaturePadding.Pkcs1);

    //        string signatureBase64 = Base64UrlEncode(signature);

    //        return $"{unsignedJwt}.{signatureBase64}";
    //    }

    //    private static string Base64UrlEncode(string input) =>
    //        Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(input));

    //    private static string Base64UrlEncode(byte[] input) =>
    //        Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');


    //}

    //public interface IGoogleWalletService
    //{
    //    string CreateEventTicketPassAsync(Ticket ticket);
    //    Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate);
    //} 
    #endregion

    #region V02
    //public interface IGoogleWalletService
    //{
    //    Task<string> CreateEventTicketPassAsync(Ticket ticket);
    //    Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, DateTime eventDate);
    //}

    //public class GoogleWalletService : IGoogleWalletService
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly HttpClient _httpClient;

    //    public GoogleWalletService(IConfiguration configuration, HttpClient httpClient)
    //    {
    //        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    //        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    //    }

    //    public async Task<string> CreateEventTicketPassAsync(Ticket ticket)
    //    {
    //        if (ticket == null) throw new ArgumentNullException(nameof(ticket));

    //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
    //        var classId = _configuration["GoogleWalletSettings:ClassId"];
    //        var passObject = new
    //        {
    //            id = $"{issuerId}.{ticket.TicketNumber}",
    //            classId = $"{issuerId}.{classId}",
    //            state = "ACTIVE",
    //            eventTicketObject = new
    //            {
    //                id = $"{issuerId}.{ticket.TicketNumber}",
    //                classReference = new { id = $"{issuerId}.event_ticket_class" },
    //                state = "ACTIVE",
    //                ticketHolderName = ticket.MemberName,
    //                ticketNumber = ticket.TicketNumber,
    //                barcode = new
    //                {
    //                    type = "QR_CODE",
    //                    value = ticket.QRCode,
    //                    alternateText = ticket.TicketNumber
    //                },
    //                seatInfo = new
    //                {
    //                    seat = new { seatNumber = ticket.MembershipNumber }
    //                },
    //                ticketType = new
    //                {
    //                    localizedValue = new
    //                    {
    //                        defaultValue = new
    //                        {
    //                            language = "en-US",
    //                            value = "General Admission"
    //                        }
    //                    }
    //                },
    //                validTimeInterval = new
    //                {
    //                    start = new { date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //                    end = new { date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
    //                }
    //            }
    //        };

    //        var jwt = await GenerateGoogleWalletJWTAsync(passObject);
    //        return $"https://pay.google.com/gp/v/save/{jwt}";
    //    }

    //    public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, DateTime eventDate)
    //    {
    //        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required.", nameof(eventName));
    //        if (string.IsNullOrWhiteSpace(eventLocation)) throw new ArgumentException("Event location is required.", nameof(eventLocation));

    //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
    //        var issuerName = _configuration["GoogleWalletSettings:IssuerName"];
    //        var classObject = new
    //        {
    //            id = $"{issuerId}.event_ticket_class",
    //            issuerName = issuerName,
    //            eventName = new
    //            {
    //                defaultValue = new
    //                {
    //                    language = "en-US",
    //                    value = eventName
    //                }
    //            },
    //            venue = new
    //            {
    //                name = new
    //                {
    //                    defaultValue = new
    //                    {
    //                        language = "en-US",
    //                        value = eventLocation
    //                    }
    //                }
    //            },
    //            dateTime = new
    //            {
    //                start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
    //            },
    //            reviewStatus = "UNDER_REVIEW"
    //        };

    //        var jwt = await GenerateGoogleWalletJWTAsync(classObject);
    //        return jwt;
    //    }

    //    private async Task<string> GenerateGoogleWalletJWTAsync(object payload)
    //    {
    //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];
    //        if (string.IsNullOrWhiteSpace(serviceAccountKeyPath)) throw new InvalidOperationException("Service account key path is not configured.");

    //        var credential = GoogleCredential.FromFile(serviceAccountKeyPath)
    //            .UnderlyingCredential as ServiceAccountCredential;
    //        if (credential == null || credential.Key == null) throw new InvalidOperationException("Invalid service account credential or missing private key.");

    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var key = new RsaSecurityKey(credential.Key);
    //        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Issuer = credential.Id,
    //            Audience = "google",
    //            Claims = new Dictionary<string, object>
    //            {
    //                { "typ", "savetowallet" },
    //                { "payload", payload },
    //                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
    //                { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() }
    //            },
    //            Expires = DateTime.UtcNow.AddHours(1),
    //            SigningCredentials = credentials
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        return tokenHandler.WriteToken(token);
    //    }
    //}

    #endregion

    #region v03
    //public interface IGoogleWalletService
    //{
    //    string CreateEventTicketPassAsync(Ticket ticket);
    //    Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate);
    //}

    //public class GoogleWalletService : IGoogleWalletService
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly HttpClient _httpClient;
    //    private readonly ILogger<GoogleWalletService> _logger;
    //    WalletobjectsService _walletService;

    //    public GoogleWalletService(IConfiguration configuration, HttpClient httpClient, ILogger<GoogleWalletService> logger, WalletobjectsService walletService)
    //    {
    //        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    //        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //        _walletService = walletService;
    //    }

    //    public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate)
    //    {
    //        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required", nameof(eventName));
    //        if (string.IsNullOrWhiteSpace(eventLocation)) throw new ArgumentException("Event location is required", nameof(eventLocation));

    //        try
    //        {
    //            var classObject = new
    //            {
    //                id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class",
    //                issuerName = _configuration["GoogleWalletSettings:IssuerName"],
    //                eventName = new
    //                {
    //                    defaultValue = new
    //                    {
    //                        language = "en-US",
    //                        value = eventName
    //                    }
    //                },
    //                venue = new
    //                {
    //                    name = new
    //                    {
    //                        defaultValue = new
    //                        {
    //                            language = "en-US",
    //                            value = eventLocation
    //                        }
    //                    }
    //                },
    //                dateTime = new { start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //                reviewStatus = "UNDER_REVIEW"
    //            };

    //            var jwt = GenerateGoogleWalletJWT(classObject);
    //            return jwt;
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error creating event ticket class for event {EventName}", eventName);
    //            throw;
    //        }
    //    }

    //    public string CreateEventTicketPassAsync(Ticket ticket)
    //    {
    //        ValidateTicket(ticket);

    //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
    //        var classId = $"{issuerId}.{_configuration["GoogleWalletSettings:ClassId"]}";
    //        var objectId = $"{issuerId}.{ticket.TicketNumber}";

    //        var payload = new
    //        {
    //            eventTicketObjects = new[]
    //            {
    //            new
    //            {
    //                id = objectId,
    //                classId = classId,
    //                state = "ACTIVE",
    //                ticketHolderName = string.IsNullOrWhiteSpace(ticket.MemberName) ? "Guest User" : ticket.MemberName,
    //                ticketNumber = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "T-0000" : ticket.TicketNumber,
    //                barcode = new
    //                {
    //                    type = "QR_CODE",
    //                    value = string.IsNullOrWhiteSpace(ticket.QRCode) ? "DEFAULT-QR-CODE" : ticket.QRCode,
    //                    alternateText = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "Ticket" : ticket.TicketNumber
    //                },
    //                seatInfo = new
    //                {
    //                    seat = new { seatNumber = string.IsNullOrWhiteSpace(ticket.MembershipNumber) ? "N/A" : ticket.MembershipNumber }
    //                },
    //                ticketType = new
    //                {
    //                    localizedValue = new
    //                    {
    //                        defaultValue = new
    //                        {
    //                            language = "en-US",
    //                            value = "General Admission"
    //                        }
    //                    }
    //                },
    //                validTimeInterval = new
    //                {
    //                    start = new { date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //                    end = new { date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
    //                }
    //            }
    //        }
    //        };

    //        var jwt = GenerateGoogleWalletJWT(payload);
    //        return $"https://pay.google.com/gp/v/save/{jwt}";
    //    }

    //    private void ValidateTicket(Ticket ticket)
    //    {
    //        if (ticket == null)
    //            throw new ArgumentNullException(nameof(ticket), "Ticket cannot be null");

    //        if (string.IsNullOrWhiteSpace(ticket.TicketNumber))
    //            throw new ArgumentException("TicketNumber is required");

    //        if (ticket.PurchaseDate == default)
    //            throw new ArgumentException("PurchaseDate is required");

    //        if (ticket.ExpiryDate == default)
    //            throw new ArgumentException("ExpiryDate is required");
    //    }

    //    private string GenerateGoogleWalletJWT(object payload)
    //    {
    //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];

    //        var credential = (ServiceAccountCredential)GoogleCredential
    //            .FromFile(serviceAccountKeyPath)
    //            .UnderlyingCredential;

    //        var header = new { alg = "RS256", typ = "JWT" };
    //        var claims = new
    //        {
    //            iss = credential.Id,  // service account email
    //            aud = "google",
    //            typ = "savetowallet",
    //            payload = payload,
    //            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    //            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
    //        };

    //        string headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
    //        string claimsBase64 = Base64UrlEncode(JsonSerializer.Serialize(claims));
    //        string unsignedJwt = $"{headerBase64}.{claimsBase64}";

    //        using var rsa = credential.Key;
    //        var signature = rsa.SignData(
    //            Encoding.UTF8.GetBytes(unsignedJwt),
    //            HashAlgorithmName.SHA256,
    //            RSASignaturePadding.Pkcs1);

    //        string signatureBase64 = Base64UrlEncode(signature);
    //        return $"{unsignedJwt}.{signatureBase64}";
    //    }

    //    private static string Base64UrlEncode(string input) =>
    //        Base64UrlEncode(Encoding.UTF8.GetBytes(input));

    //    private static string Base64UrlEncode(byte[] input) =>
    //        Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    //}

    #endregion


    //public class GoogleWalletService : IGoogleWalletService
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly HttpClient _httpClient;
    //    private readonly ILogger<GoogleWalletService> _logger;
    //    WalletobjectsService _walletService;

    //    public GoogleWalletService(
    //IConfiguration configuration,
    //HttpClient httpClient,
    //ILogger<GoogleWalletService> logger)
    //    {
    //        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    //        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];
    //        if (string.IsNullOrWhiteSpace(serviceAccountKeyPath))
    //            throw new InvalidOperationException("GoogleWalletSettings:ServiceAccountKeyPath is required in configuration");

    //        var credential = GoogleCredential
    //            .FromFile(serviceAccountKeyPath)
    //            .CreateScoped(WalletobjectsService.ScopeConstants.WalletObjectIssuer);

    //        _walletService = new WalletobjectsService(new BaseClientService.Initializer()
    //        {
    //            HttpClientInitializer = credential,
    //            ApplicationName = "TicketingSystem"
    //        });
    //    }


    //    //    public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate)
    //    //    {
    //    //        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required", nameof(eventName));
    //    //        if (string.IsNullOrWhiteSpace(eventLocation)) throw new ArgumentException("Event location is required", nameof(eventLocation));

    //    //        try
    //    //        {
    //    //            var safeEventName = string.IsNullOrWhiteSpace(eventName) ? "Event" : eventName.Trim();
    //    //            var safeEventLocation = string.IsNullOrWhiteSpace(eventLocation) ? "Event Location" : eventLocation.Trim();
    //    //            var safeIssuerName = _configuration["GoogleWalletSettings:IssuerName"];
    //    //            if (string.IsNullOrWhiteSpace(safeIssuerName))
    //    //            {
    //    //                safeIssuerName = "Event Organizer";
    //    //            }

    //    //            var classObject = new
    //    //            {
    //    //                id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class",
    //    //                issuerName = safeIssuerName,
    //    //                eventName = new
    //    //                {
    //    //                    defaultValue = new
    //    //                    {
    //    //                        language = "en-US",
    //    //                        value = safeEventName
    //    //                    }
    //    //                },
    //    //                venue = new
    //    //                {
    //    //                    name = new
    //    //                    {
    //    //                        defaultValue = new
    //    //                        {
    //    //                            language = "en-US",
    //    //                            value = safeEventLocation
    //    //                        }
    //    //                    }
    //    //                },
    //    //                dateTime = new { start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //    //                reviewStatus = "UNDER_REVIEW"
    //    //            };

    //    //            var jwt = GenerateGoogleWalletJWT(classObject);
    //    //            return jwt;
    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            _logger.LogError(ex, "Error creating event ticket class for event {EventName}", eventName);
    //    //            throw;
    //    //        }
    //    //    }

    //    //    public string CreateEventTicketPassAsync(Ticket ticket)
    //    //    {
    //    //        ValidateTicket(ticket);

    //    //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
    //    //        var classId = $"{issuerId}.{_configuration["GoogleWalletSettings:ClassId"]}";
    //    //        var objectId = $"{issuerId}.{ticket.TicketNumber}";

    //    //        var safeTicketHolderName = string.IsNullOrWhiteSpace(ticket.MemberName) ? "Guest User" : ticket.MemberName.Trim();
    //    //        var safeTicketNumber = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "T-0000" : ticket.TicketNumber.Trim();
    //    //        var safeQRCode = string.IsNullOrWhiteSpace(ticket.QRCode) ? $"TICKET-{safeTicketNumber}" : ticket.QRCode.Trim();
    //    //        var safeSeatNumber = string.IsNullOrWhiteSpace(ticket.MembershipNumber) ? "General Admission" : ticket.MembershipNumber.Trim();

    //    //        var payload = new
    //    //        {
    //    //            eventTicketObjects = new[]
    //    //            {
    //    //            new
    //    //            {
    //    //                id = objectId,
    //    //                classId = classId,
    //    //                state = "ACTIVE",
    //    //                ticketHolderName = safeTicketHolderName,
    //    //                ticketNumber = safeTicketNumber,
    //    //                barcode = new
    //    //                {
    //    //                    type = "QR_CODE",
    //    //                    value = safeQRCode,
    //    //                    alternateText = safeTicketNumber
    //    //                },
    //    //                seatInfo = new
    //    //                {
    //    //                    seat = new { seatNumber = safeSeatNumber }
    //    //                },
    //    //                ticketType = new
    //    //                {
    //    //                    localizedValue = new
    //    //                    {
    //    //                        defaultValue = new
    //    //                        {
    //    //                            language = "en-US",
    //    //                            value = "General Admission" // Always provide a non-empty default value
    //    //                        }
    //    //                    }
    //    //                },
    //    //                validTimeInterval = new
    //    //                {
    //    //                    start = new { date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //    //                    end = new { date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
    //    //                }
    //    //            }
    //    //        }
    //    //        };

    //    //        var jwt = GenerateGoogleWalletJWT(payload);
    //    //        return $"https://pay.google.com/gp/v/save/{jwt}";
    //    //    }

    //    //    private void ValidateTicket(Ticket ticket)
    //    //    {
    //    //        if (ticket == null)
    //    //            throw new ArgumentNullException(nameof(ticket), "Ticket cannot be null");

    //    //        if (string.IsNullOrWhiteSpace(ticket.TicketNumber))
    //    //            throw new ArgumentException("TicketNumber is required and cannot be empty");

    //    //        if (ticket.PurchaseDate == default)
    //    //            throw new ArgumentException("PurchaseDate is required");

    //    //        if (ticket.ExpiryDate == default)
    //    //            throw new ArgumentException("ExpiryDate is required");

    //    //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
    //    //        var classId = _configuration["GoogleWalletSettings:ClassId"];

    //    //        if (string.IsNullOrWhiteSpace(issuerId))
    //    //            throw new InvalidOperationException("GoogleWalletSettings:IssuerId is required in configuration");

    //    //        if (string.IsNullOrWhiteSpace(classId))
    //    //            throw new InvalidOperationException("GoogleWalletSettings:ClassId is required in configuration");
    //    //    }

    //    //    private string GenerateGoogleWalletJWT(object payload)
    //    //    {
    //    //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];

    //    //        if (string.IsNullOrWhiteSpace(serviceAccountKeyPath))
    //    //            throw new InvalidOperationException("GoogleWalletSettings:ServiceAccountKeyPath is required in configuration");

    //    //        var credential = (ServiceAccountCredential)GoogleCredential
    //    //            .FromFile(serviceAccountKeyPath)
    //    //            .UnderlyingCredential;

    //    //        var header = new { alg = "RS256", typ = "JWT" };
    //    //        var claims = new
    //    //        {
    //    //            iss = credential.Id,  // service account email
    //    //            aud = "google",
    //    //            typ = "savetowallet",
    //    //            payload = payload,
    //    //            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    //    //            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
    //    //        };

    //    //        string headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
    //    //        string claimsBase64 = Base64UrlEncode(JsonSerializer.Serialize(claims));
    //    //        string unsignedJwt = $"{headerBase64}.{claimsBase64}";

    //    //        using var rsa = credential.Key;
    //    //        var signature = rsa.SignData(
    //    //            Encoding.UTF8.GetBytes(unsignedJwt),
    //    //            HashAlgorithmName.SHA256,
    //    //            RSASignaturePadding.Pkcs1);

    //    //        string signatureBase64 = Base64UrlEncode(signature);
    //    //        return $"{unsignedJwt}.{signatureBase64}";
    //    //    }

    //    //    private static string Base64UrlEncode(string input) =>
    //    //        Base64UrlEncode(Encoding.UTF8.GetBytes(input));

    //    //    private static string Base64UrlEncode(byte[] input) =>
    //    //        Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    //    //

    //    public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate)
    //    {
    //        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required", nameof(eventName));
    //        if (string.IsNullOrWhiteSpace(eventLocation)) throw new ArgumentException("Event location is required", nameof(eventLocation));

    //        try
    //        {
    //            var classObject = new
    //            {
    //                id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class",
    //                issuerName = _configuration["GoogleWalletSettings:IssuerName"],
    //                eventName = new
    //                {
    //                    defaultValue = new
    //                    {
    //                        language = "en-US",
    //                        value = eventName
    //                    }
    //                },
    //                venue = new
    //                {
    //                    name = new
    //                    {
    //                        defaultValue = new
    //                        {
    //                            language = "en-US",
    //                            value = eventLocation
    //                        }
    //                    }
    //                },
    //                dateTime = new { start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //                reviewStatus = "UNDER_REVIEW"
    //            };

    //            var jwt = GenerateGoogleWalletJWT(classObject);
    //            return jwt;
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error creating event ticket class for event {EventName}", eventName);
    //            throw;
    //        }
    //    }

    //    public string CreateEventTicketPassAsync(Ticket ticket)
    //    {
    //        ValidateTicket(ticket);

    //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
    //        var classId = $"{issuerId}.{_configuration["GoogleWalletSettings:ClassId"]}";
    //        var objectId = $"{issuerId}.{ticket.TicketNumber}";

    //        var payload = new
    //        {
    //            eventTicketObjects = new[]
    //            {
    //           new
    //           {
    //               id = objectId,
    //               classId = classId,
    //               state = "ACTIVE",
    //               ticketHolderName = string.IsNullOrWhiteSpace(ticket.MemberName) ? "Guest User" : ticket.MemberName,
    //               ticketNumber = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "T-0000" : ticket.TicketNumber,
    //               barcode = new
    //               {
    //                   type = "QR_CODE",
    //                   value = string.IsNullOrWhiteSpace(ticket.QRCode) ? "DEFAULT-QR-CODE" : ticket.QRCode,
    //                   alternateText = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "Ticket" : ticket.TicketNumber
    //               },
    //               seatInfo = new
    //               {
    //                   seat = new { seatNumber = string.IsNullOrWhiteSpace(ticket.MembershipNumber) ? "N/A" : ticket.MembershipNumber }
    //               },
    //               ticketType = new
    //               {
    //                   localizedValue = new
    //                   {
    //                       defaultValue = new
    //                       {
    //                           language = "en-US",
    //                           value = "General Admission"
    //                       }
    //                   }
    //               },
    //               validTimeInterval = new
    //               {
    //                   start = new { date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
    //                   end = new { date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
    //               }
    //           }
    //       }
    //        };

    //        var jwt = GenerateGoogleWalletJWT(payload);
    //        return $"https://pay.google.com/gp/v/save/{jwt}";
    //    }

    //    private void ValidateTicket(Ticket ticket)
    //    {
    //        if (ticket == null)
    //            throw new ArgumentNullException(nameof(ticket), "Ticket cannot be null");

    //        if (string.IsNullOrWhiteSpace(ticket.TicketNumber))
    //            throw new ArgumentException("TicketNumber is required");

    //        if (ticket.PurchaseDate == default)
    //            throw new ArgumentException("PurchaseDate is required");

    //        if (ticket.ExpiryDate == default)
    //            throw new ArgumentException("ExpiryDate is required");
    //    }

    //    private string GenerateGoogleWalletJWT(object payload)
    //    {
    //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];

    //        var credential = (ServiceAccountCredential)GoogleCredential
    //            .FromFile(serviceAccountKeyPath)
    //            .UnderlyingCredential;

    //        var header = new { alg = "RS256", typ = "JWT" };
    //        var claims = new
    //        {
    //            iss = credential.Id,  // service account email
    //            aud = "google",
    //            typ = "savetowallet",
    //            payload = payload,
    //            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    //            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
    //        };

    //        string headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
    //        string claimsBase64 = Base64UrlEncode(JsonSerializer.Serialize(claims));
    //        string unsignedJwt = $"{headerBase64}.{claimsBase64}";

    //        using var rsa = credential.Key;
    //        var signature = rsa.SignData(
    //            Encoding.UTF8.GetBytes(unsignedJwt),
    //            HashAlgorithmName.SHA256,
    //            RSASignaturePadding.Pkcs1);

    //        string signatureBase64 = Base64UrlEncode(signature);
    //        return $"{unsignedJwt}.{signatureBase64}";
    //    }

    //    private static string Base64UrlEncode(string input) =>
    //        Base64UrlEncode(Encoding.UTF8.GetBytes(input));

    //    private static string Base64UrlEncode(byte[] input) =>
    //        Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');


    //}

    #region v04

    public interface IGoogleWalletService
    {
        string CreateEventTicketPassAsync(Ticket ticket);
        Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate);
    }
    public class GoogleWalletService : IGoogleWalletService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleWalletService> _logger;
        WalletobjectsService _walletService;

        public GoogleWalletService(
    IConfiguration configuration,
    HttpClient httpClient,
    ILogger<GoogleWalletService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];
            if (string.IsNullOrWhiteSpace(serviceAccountKeyPath))
                throw new InvalidOperationException("GoogleWalletSettings:ServiceAccountKeyPath is required in configuration");

            var credential = GoogleCredential
                .FromFile(serviceAccountKeyPath)
                .CreateScoped(WalletobjectsService.ScopeConstants.WalletObjectIssuer);

            _walletService = new WalletobjectsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "TicketingSystem"
            });
        }


        //    public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate)
        //    {
        //        if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required", nameof(eventName));
        //        if (string.IsNullOrWhiteSpace(eventLocation)) throw new ArgumentException("Event location is required", nameof(eventLocation));

        //        try
        //        {
        //            var safeEventName = string.IsNullOrWhiteSpace(eventName) ? "Event" : eventName.Trim();
        //            var safeEventLocation = string.IsNullOrWhiteSpace(eventLocation) ? "Event Location" : eventLocation.Trim();
        //            var safeIssuerName = _configuration["GoogleWalletSettings:IssuerName"];
        //            if (string.IsNullOrWhiteSpace(safeIssuerName))
        //            {
        //                safeIssuerName = "Event Organizer";
        //            }

        //            var classObject = new
        //            {
        //                id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class",
        //                issuerName = safeIssuerName,
        //                eventName = new
        //                {
        //                    defaultValue = new
        //                    {
        //                        language = "en-US",
        //                        value = safeEventName
        //                    }
        //                },
        //                venue = new
        //                {
        //                    name = new
        //                    {
        //                        defaultValue = new
        //                        {
        //                            language = "en-US",
        //                            value = safeEventLocation
        //                        }
        //                    }
        //                },
        //                dateTime = new { start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
        //                reviewStatus = "UNDER_REVIEW"
        //            };

        //            var jwt = GenerateGoogleWalletJWT(classObject);
        //            return jwt;
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error creating event ticket class for event {EventName}", eventName);
        //            throw;
        //        }
        //    }

        //    public string CreateEventTicketPassAsync(Ticket ticket)
        //    {
        //        ValidateTicket(ticket);

        //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
        //        var classId = $"{issuerId}.{_configuration["GoogleWalletSettings:ClassId"]}";
        //        var objectId = $"{issuerId}.{ticket.TicketNumber}";

        //        var safeTicketHolderName = string.IsNullOrWhiteSpace(ticket.MemberName) ? "Guest User" : ticket.MemberName.Trim();
        //        var safeTicketNumber = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "T-0000" : ticket.TicketNumber.Trim();
        //        var safeQRCode = string.IsNullOrWhiteSpace(ticket.QRCode) ? $"TICKET-{safeTicketNumber}" : ticket.QRCode.Trim();
        //        var safeSeatNumber = string.IsNullOrWhiteSpace(ticket.MembershipNumber) ? "General Admission" : ticket.MembershipNumber.Trim();

        //        var payload = new
        //        {
        //            eventTicketObjects = new[]
        //            {
        //            new
        //            {
        //                id = objectId,
        //                classId = classId,
        //                state = "ACTIVE",
        //                ticketHolderName = safeTicketHolderName,
        //                ticketNumber = safeTicketNumber,
        //                barcode = new
        //                {
        //                    type = "QR_CODE",
        //                    value = safeQRCode,
        //                    alternateText = safeTicketNumber
        //                },
        //                seatInfo = new
        //                {
        //                    seat = new { seatNumber = safeSeatNumber }
        //                },
        //                ticketType = new
        //                {
        //                    localizedValue = new
        //                    {
        //                        defaultValue = new
        //                        {
        //                            language = "en-US",
        //                            value = "General Admission" // Always provide a non-empty default value
        //                        }
        //                    }
        //                },
        //                validTimeInterval = new
        //                {
        //                    start = new { date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
        //                    end = new { date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
        //                }
        //            }
        //        }
        //        };

        //        var jwt = GenerateGoogleWalletJWT(payload);
        //        return $"https://pay.google.com/gp/v/save/{jwt}";
        //    }

        //    private void ValidateTicket(Ticket ticket)
        //    {
        //        if (ticket == null)
        //            throw new ArgumentNullException(nameof(ticket), "Ticket cannot be null");

        //        if (string.IsNullOrWhiteSpace(ticket.TicketNumber))
        //            throw new ArgumentException("TicketNumber is required and cannot be empty");

        //        if (ticket.PurchaseDate == default)
        //            throw new ArgumentException("PurchaseDate is required");

        //        if (ticket.ExpiryDate == default)
        //            throw new ArgumentException("ExpiryDate is required");

        //        var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
        //        var classId = _configuration["GoogleWalletSettings:ClassId"];

        //        if (string.IsNullOrWhiteSpace(issuerId))
        //            throw new InvalidOperationException("GoogleWalletSettings:IssuerId is required in configuration");

        //        if (string.IsNullOrWhiteSpace(classId))
        //            throw new InvalidOperationException("GoogleWalletSettings:ClassId is required in configuration");
        //    }

        //    private string GenerateGoogleWalletJWT(object payload)
        //    {
        //        var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];

        //        if (string.IsNullOrWhiteSpace(serviceAccountKeyPath))
        //            throw new InvalidOperationException("GoogleWalletSettings:ServiceAccountKeyPath is required in configuration");

        //        var credential = (ServiceAccountCredential)GoogleCredential
        //            .FromFile(serviceAccountKeyPath)
        //            .UnderlyingCredential;

        //        var header = new { alg = "RS256", typ = "JWT" };
        //        var claims = new
        //        {
        //            iss = credential.Id,  // service account email
        //            aud = "google",
        //            typ = "savetowallet",
        //            payload = payload,
        //            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        //            exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        //        };

        //        string headerBase64 = Base64UrlEncode(JsonSerializer.Serialize(header));
        //        string claimsBase64 = Base64UrlEncode(JsonSerializer.Serialize(claims));
        //        string unsignedJwt = $"{headerBase64}.{claimsBase64}";

        //        using var rsa = credential.Key;
        //        var signature = rsa.SignData(
        //            Encoding.UTF8.GetBytes(unsignedJwt),
        //            HashAlgorithmName.SHA256,
        //            RSASignaturePadding.Pkcs1);

        //        string signatureBase64 = Base64UrlEncode(signature);
        //        return $"{unsignedJwt}.{signatureBase64}";
        //    }

        //    private static string Base64UrlEncode(string input) =>
        //        Base64UrlEncode(Encoding.UTF8.GetBytes(input));

        //    private static string Base64UrlEncode(byte[] input) =>
        //        Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        //

        public async Task<string> CreateEventTicketClassAsync(string eventName, string eventLocation, System.DateTime eventDate)
        {
            if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required", nameof(eventName));
            if (string.IsNullOrWhiteSpace(eventLocation)) throw new ArgumentException("Event location is required", nameof(eventLocation));

            try
            {
                var classObject = new
                {
                    id = $"{_configuration["GoogleWalletSettings:IssuerId"]}.event_ticket_class",
                    issuerName = _configuration["GoogleWalletSettings:IssuerName"],
                    eventName = new
                    {
                        defaultValue = new
                        {
                            language = "en-US",
                            value = eventName
                        }
                    },
                    venue = new
                    {
                        name = new
                        {
                            defaultValue = new
                            {
                                language = "en-US",
                                value = eventLocation
                            }
                        }
                    },
                    dateTime = new { start = eventDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    reviewStatus = "UNDER_REVIEW"
                };

                var jwt = GenerateGoogleWalletJWT(classObject);
                return jwt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event ticket class for event {EventName}", eventName);
                throw;
            }
        }

        public string CreateEventTicketPassAsync(Ticket ticket)
        {
            ValidateTicket(ticket);

            var issuerId = _configuration["GoogleWalletSettings:IssuerId"];
            var classId = $"{issuerId}.{_configuration["GoogleWalletSettings:ClassId"]}";
            var objectId = $"{issuerId}.{ticket.TicketNumber}";

            var payload = new
            {
                eventTicketObjects = new[]
                {
               new
               {
                   id = objectId,
                   classId = classId,
                   state = "ACTIVE",
                   ticketHolderName = string.IsNullOrWhiteSpace(ticket.MemberName) ? "Guest User" : ticket.MemberName,
                   ticketNumber = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "T-0000" : ticket.TicketNumber,
                   barcode = new
                   {
                       type = "QR_CODE",
                       value = string.IsNullOrWhiteSpace(ticket.QRCode) ? "DEFAULT-QR-CODE" : ticket.QRCode,
                       alternateText = string.IsNullOrWhiteSpace(ticket.TicketNumber) ? "Ticket" : ticket.TicketNumber
                   },
                   seatInfo = new
                   {
                       seat = new { seatNumber = string.IsNullOrWhiteSpace(ticket.MembershipNumber) ? "N/A" : ticket.MembershipNumber }
                   },
                   ticketType = new
                   {
                       localizedValue = new
                       {
                           defaultValue = new
                           {
                               language = "en-US",
                               value = "General Admission"
                           }
                       }
                   },
                   validTimeInterval = new
                   {
                       start = new { date = ticket.PurchaseDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                       end = new { date = ticket.ExpiryDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                   }
               }
           }
            };

            var jwt = GenerateGoogleWalletJWT(payload);
            return $"https://pay.google.com/gp/v/save/{jwt}";
        }

        private void ValidateTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket), "Ticket cannot be null");

            if (string.IsNullOrWhiteSpace(ticket.TicketNumber))
                throw new ArgumentException("TicketNumber is required");

            if (ticket.PurchaseDate == default)
                throw new ArgumentException("PurchaseDate is required");

            if (ticket.ExpiryDate == default)
                throw new ArgumentException("ExpiryDate is required");
        }

        private string GenerateGoogleWalletJWT(object payload)
        {
            var serviceAccountKeyPath = _configuration["GoogleWalletSettings:ServiceAccountKeyPath"];

            var credential = (ServiceAccountCredential)GoogleCredential
                .FromFile(serviceAccountKeyPath)
                .UnderlyingCredential;

            var header = new { alg = "RS256", typ = "JWT" };
            var claims = new
            {
                iss = credential.Id,  // service account email
                aud = "google",
                typ = "savetowallet",
                payload = payload,
                iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
            };

            string headerBase64 = Base64UrlEncode(System.Text.Json.JsonSerializer.Serialize(header));
            string claimsBase64 = Base64UrlEncode(System.Text.Json.JsonSerializer.Serialize(claims));
            string unsignedJwt = $"{headerBase64}.{claimsBase64}";

            using var rsa = credential.Key;
            var signature = rsa.SignData(
                Encoding.UTF8.GetBytes(unsignedJwt),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            string signatureBase64 = Base64UrlEncode(signature);
            return $"{unsignedJwt}.{signatureBase64}";
        }

        private static string Base64UrlEncode(string input) =>
            Base64UrlEncode(Encoding.UTF8.GetBytes(input));

        private static string Base64UrlEncode(byte[] input) =>
            Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');


    }

    #endregion







    #region 50%

    public class GoogleWalletService2
    {
        private readonly WalletobjectsService _service;
        private readonly ServiceAccountCredential _credentials;
        private readonly string _issuerId;
        private readonly string _classId;

        public GoogleWalletService2(IConfiguration configuration)
        {
            var settings = configuration.GetSection("GoogleWalletSettings");
            var keyFilePath = settings["ServiceAccountKeyPath"];
            _issuerId = settings["IssuerId"];
            _classId = settings["ClassId"];

            _credentials = (ServiceAccountCredential)GoogleCredential
                .FromFile(keyFilePath)
                .CreateScoped(WalletobjectsService.ScopeConstants.WalletObjectIssuer)
                .UnderlyingCredential;

            _service = new WalletobjectsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credentials
            });
        }

        /// <summary>
        /// Ensure the event class exists in Google Wallet.
        /// </summary>
        private string EnsureClassExists()
        {
            string classId = $"{_issuerId}.{_classId}";
            try
            {
                _service.Eventticketclass.Get(classId).Execute();
                return classId; // Already exists
            }
            catch
            {
                var newClass = new EventTicketClass
                {
                    Id = classId,
                    EventName = new LocalizedString
                    {
                        DefaultValue = new TranslatedString
                        {
                            Language = "en-US",
                            Value = "Travel Ticket"
                        }
                    },
                    IssuerName = "Your Company",
                    ReviewStatus = "UNDER_REVIEW"
                };

                _service.Eventticketclass.Insert(newClass).Execute();
                return classId;
            }
        }

        /// <summary>
        /// Generate an "Add to Google Wallet" link for a specific ticket.
        /// </summary>
        public string GenerateWalletLink(Ticket ticket)
        {
            string classId = EnsureClassExists();
            string objectId = $"{_issuerId}.ticket_{ticket.Id}";

            // Build the EventTicketObject for this Ticket
            var ticketObject = new EventTicketObject
            {
                Id = objectId,
                ClassId = classId,
                State = "ACTIVE",
                HeroImage = new Image
                {
                    SourceUri = new ImageUri { Uri = "https://yourcdn.com/logo.png" },
                    ContentDescription = new LocalizedString
                    {
                        DefaultValue = new TranslatedString { Language = "en-US", Value = "Ticket Image" }
                    }
                },
                Barcode = new Barcode
                {
                    Type = "QR_CODE",
                    Value = ticket.QRCode
                },
                TicketHolderName = ticket.MemberName,
                TicketNumber = ticket.TicketNumber,
                ValidTimeInterval = new TimeInterval
                {
                    Start = new Google.Apis.Walletobjects.v1.Data.DateTime
                    {
                        Date = ticket.PurchaseDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                    },
                    End = new Google.Apis.Walletobjects.v1.Data.DateTime
                    {
                        Date = ticket.ExpiryDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                    }
                }
            };

            // JWT payload
            var payload = new
            {
                iss = _credentials.Id,
                aud = "google",
                origins = new[] { "https://yourwebsite.com" },
                typ = "savetowallet",
                payload = new
                {
                    eventTicketObjects = new[] { ticketObject }
                }
            };

            string token;
            {
                var jwtPayload = JwtPayload.Deserialize(JsonConvert.SerializeObject(payload));
                var key = new RsaSecurityKey(_credentials.Key);
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
                var jwt = new JwtSecurityToken(new JwtHeader(signingCredentials), jwtPayload);
                token = new JwtSecurityTokenHandler().WriteToken(jwt);
            }

            return $"https://pay.google.com/gp/v/save/{token}";
        }
    }


    #endregion


    #region v05 100%

    public class GoogleWalletService3 : IGoogleWalletService3
    {
        private ServiceAccountCredential credentials;
        private WalletobjectsService service;
        private readonly string issuerId;
        private readonly string classSuffix;
        private readonly string keyFilePath;
        private IHostingEnvironment _env;
        public GoogleWalletService3(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            var walletSettings = configuration.GetSection("GoogleWalletSettings");

            //string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "GoogleKeys", "natchibaech-793937ea412e.json");
            //keyFilePath = Path.Combine(_env.WebRootPath, "GoogleKeys", "natchibaech-9e515a4f00be.json");// walletSettings["ServiceAccountKeyPath"] ;
            keyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "GoogleKeys", "natchibaech-470810-cfae0204bb79.json"); ;// walletSettings["ServiceAccountKeyPath"] ;
            issuerId = walletSettings["IssuerId"];
            classSuffix = walletSettings["ClassId"];
            Auth();
            //_env = env;
        }

        // Existing Auth method
        private void Auth()
        {
            credentials = (ServiceAccountCredential)GoogleCredential
                .FromFile(keyFilePath)
                .CreateScoped(
                new[] {
                WalletobjectsService.ScopeConstants.WalletObjectIssuer
                    //WalletobjectsService.ScopeConstants.WalletObjects
}
                //new List<string>
                //{
                //WalletobjectsService.ScopeConstants.WalletObjectIssuer
                //}
                )
                .UnderlyingCredential;

            service = new WalletobjectsService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials
                });
        }

        //// Existing CreateClass method
        //private string CreateClass()
        //{
        //    Stream responseStream = service.Eventticketclass
        //        .Get($"{issuerId}.{classSuffix}")
        //        .ExecuteAsStream();

        //    StreamReader responseReader = new StreamReader(responseStream);
        //    JObject jsonResponse = JObject.Parse(responseReader.ReadToEnd());

        //    if (!jsonResponse.ContainsKey("error"))
        //    {
        //        Console.WriteLine($"Class {issuerId}.{classSuffix} already exists!");
        //        return $"{issuerId}.{classSuffix}";
        //    }
        //    else if (jsonResponse["error"].Value<int>("code") != 404)
        //    {
        //        Console.WriteLine(jsonResponse.ToString());
        //        return $"{issuerId}.{classSuffix}";
        //    }

        //    EventTicketClass newClass = new EventTicketClass
        //    {
        //        EventId = $"{issuerId}.{classSuffix}",
        //        EventName = new LocalizedString
        //        {
        //            DefaultValue = new TranslatedString
        //            {
        //                Language = "en-US",
        //                Value = "Event Ticket"
        //            }
        //        },
        //        Id = $"{issuerId}.{classSuffix}",
        //        IssuerName = "Travel Site",
        //        ReviewStatus = "UNDER_REVIEW",//SUBMIT -- UNDER_REVIEW
        //        // Optional: Add more class properties like logo, venue, etc.
        //        Venue = new EventVenue
        //        {
        //            Name = new LocalizedString
        //            {
        //                DefaultValue = new TranslatedString
        //                {
        //                    Language = "en-US",
        //                    Value = "Event Venue"
        //                }
        //            },
        //            Address = new LocalizedString
        //            {
        //                DefaultValue = new TranslatedString
        //                {
        //                    Language = "en-US",
        //                    Value = "123 Event St, City, Country"
        //                }
        //            }
        //        }
        //    };

        //    responseStream = service.Eventticketclass
        //        .Insert(newClass)
        //        .ExecuteAsStream();

        //    responseReader = new StreamReader(responseStream);
        //    jsonResponse = JObject.Parse(responseReader.ReadToEnd());

        //    Console.WriteLine("Class insert response");
        //    Console.WriteLine(jsonResponse.ToString());

        //    return $"{issuerId}.{classSuffix}";
        //}


        //private void Auth()
        //{
        //    //string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "GoogleKeys", "natchibaech-793937ea412e.json");
        //    credentials=  (ServiceAccountCredential) GoogleCredential.FromFile(keyFilePath)
        //        .CreateScoped(WalletobjectsService.ScopeConstants.WalletObjectIssuer);

        //    //googleCredential = GoogleCredential
        //    //    .FromFile(keyFilePath)
        //    //    .CreateScoped(new[] {
        //    //    WalletobjectsService.ScopeConstants.WalletObjectIssuer
        //    //    });

        //    service = new WalletobjectsService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "Travel Site"
        //    });
        //}



        private string CreateClass()
        {
            try
            {
                // Try to fetch existing class
                var existingClass = service.Eventticketclass
                    .Get($"{issuerId}.{classSuffix}")
                    .Execute();

                Console.WriteLine($"Class {existingClass.Id} already exists!");
                return existingClass.Id;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Class not found → create a new one
                Console.WriteLine($"Class {issuerId}.{classSuffix} not found. Creating new class...");

                var newClass = new EventTicketClass
                {
                    Id = $"{issuerId}.{classSuffix}",
                    EventName = new LocalizedString
                    {
                        DefaultValue = new TranslatedString
                        {
                            Language = "en-US",
                            Value = "Event Ticket"
                        }
                    },
                    IssuerName = "Travel Site",
                    ReviewStatus = "SUBMIT", // or "SUBMIT"
                    Venue = new EventVenue
                    {
                        Name = new LocalizedString
                        {
                            DefaultValue = new TranslatedString
                            {
                                Language = "en-US",
                                Value = "Event Venue"
                            }
                        },
                        Address = new LocalizedString
                        {
                            DefaultValue = new TranslatedString
                            {
                                Language = "en-US",
                                Value = "123 Event St, City, Country"
                            }
                        }
                    }
                };

                var insertedClass = service.Eventticketclass.Insert(newClass).Execute();
                Console.WriteLine("Class created successfully");
                Console.WriteLine($"Class ID: {insertedClass.Id}");

                return insertedClass.Id;
            }
            catch (Google.GoogleApiException ex)
            {
                // Handle other Google API errors (401, 403, etc.)
                Console.WriteLine($"Google API error: {ex.HttpStatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                Console.WriteLine($"Unexpected error creating class: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a Google Wallet EventTicketObject for a ticket and returns an "Add to Google Wallet" link.
        /// </summary>
        /// <param name="ticket">The ticket to add to Google Wallet.</param>
        /// <returns>An "Add to Google Wallet" link.</returns>
        public string CreateTicketWalletLink(Ticket ticket)
        {
            // Ensure the class exists
            string classId = CreateClass();

            // Create an EventTicketObject
            EventTicketObject ticketObject = new EventTicketObject
            {
                Id = $"{issuerId}.{ticket.TicketNumber}", // Unique ID using TicketNumber
                ClassId = classId,
                State = "ACTIVE",
                TicketHolderName = ticket.MemberName,
                TicketNumber = ticket.TicketNumber,
                //Barcode = new Barcode
                //{
                //    Type = "QR_CODE",
                //    Value = $"http://travelsite.runasp.net/api/Tickets/validate/{ticket.TicketNumber}",//ticket.QRCode, // Use the QRCode from the Ticket
                //    AlternateText = ticket.TicketNumber // Display ticket number as fallback
                //},
                TicketType = new LocalizedString
                {
                    DefaultValue = new TranslatedString
                    {
                        Language = "en-US",
                        Value = "General Admission"
                    }
                },
                ValidTimeInterval = new TimeInterval
                {
                    Start = new Google.Apis.Walletobjects.v1.Data.DateTime
                    {
                        Date = ticket.PurchaseDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                    },
                    End = new Google.Apis.Walletobjects.v1.Data.DateTime
                    {
                        Date = ticket.ExpiryDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                    }
                }
            };

            // Insert the ticket object into Google Wallet
            Stream responseStream = service.Eventticketobject
                .Insert(ticketObject)
                .ExecuteAsStream();

            StreamReader responseReader = new StreamReader(responseStream);
            JObject jsonResponse = JObject.Parse(responseReader.ReadToEnd());

            Console.WriteLine("Ticket object insert response");
            Console.WriteLine(jsonResponse.ToString());

            // Generate JWT for existing object
            return CreateJWTForTicket(ticketObject.Id, classId);
        }

        /// <summary>
        /// Generates a signed JWT for an existing EventTicketObject.
        /// </summary>
        /// <param name="objectId">The ID of the ticket object.</param>
        /// <param name="classId">The ID of the ticket class.</param>
        /// <returns>An "Add to Google Wallet" link.</returns>
        private string CreateJWTForTicket(string objectId, string classId)
        {
            JsonSerializerSettings excludeNulls = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // Create payload with the event ticket object
            Dictionary<string, object> objectsToAdd = new Dictionary<string, object>
        {
            {
                "eventTicketObjects", new List<EventTicketObject>
                {
                    new EventTicketObject
                    {
                        Id = objectId,
                        ClassId = classId
                    }
                }
            }
        };

            // Create JSON payload
            JObject serializedPayload = JObject.Parse(
                JsonConvert.SerializeObject(objectsToAdd, excludeNulls));

            // Create JWT payload
            JObject jwtPayload = JObject.Parse(JsonConvert.SerializeObject(new
            {
                iss = credentials.Id,
                aud = "google",
                origins = new string[] { "www.example.com" }, // Replace with your domain
                typ = "savetowallet",
                payload = serializedPayload
            }));

            // Deserialize into JwtPayload
            JwtPayload claims = JwtPayload.Deserialize(jwtPayload.ToString());

            // Sign the JWT
            RsaSecurityKey key = new RsaSecurityKey(credentials.Key);
            SigningCredentials signingCredentials = new SigningCredentials(
                key, SecurityAlgorithms.RsaSha256);
            JwtSecurityToken jwt = new JwtSecurityToken(
                new JwtHeader(signingCredentials), claims);
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            string walletLink = $"https://pay.google.com/gp/v/save/{token}";
            Console.WriteLine("Add to Google Wallet link");
            Console.WriteLine(walletLink);

            return walletLink;
        }

        //private string CreateJWTForTicket(string objectId, string classId)
        //{
        //    JsonSerializerSettings excludeNulls = new JsonSerializerSettings()
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    };

        //    // Create payload with the event ticket object
        //    Dictionary<string, object> objectsToAdd = new Dictionary<string, object>
        //{
        //    {
        //        "eventTicketObjects", new List<EventTicketObject>
        //        {
        //            new EventTicketObject
        //            {
        //                Id = objectId,
        //                ClassId = classId
        //            }
        //        }
        //    }
        //};

        //    // Create JSON payload
        //    JObject serializedPayload = JObject.Parse(
        //        JsonConvert.SerializeObject(objectsToAdd, excludeNulls));

        //    // Create JWT payload
        //    JObject jwtPayload = JObject.Parse(JsonConvert.SerializeObject(new
        //    {
        //        iss = credentials.Id,
        //        aud = "google",
        //        origins = new string[] { "www.example.com" }, // Replace with your domain
        //        typ = "savetowallet",
        //        payload = serializedPayload
        //    }));

        //    // Deserialize into JwtPayload
        //    JwtPayload claims = JwtPayload.Deserialize(jwtPayload.ToString());

        //    // Sign the JWT
        //    RsaSecurityKey key = new RsaSecurityKey(credentials.Key);
        //    SigningCredentials signingCredentials = new SigningCredentials(
        //        key, SecurityAlgorithms.RsaSha256);
        //    JwtSecurityToken jwt = new JwtSecurityToken(
        //        new JwtHeader(signingCredentials), claims);
        //    string token = new JwtSecurityTokenHandler().WriteToken(jwt);

        //    string walletLink = $"https://pay.google.com/gp/v/save/{token}";
        //    Console.WriteLine("Add to Google Wallet link");
        //    Console.WriteLine(walletLink);

        //    return walletLink;
        //}
    
    
    
    }



    //public class GoogleWalletService3 : IGoogleWalletService3
    //{
    //    private GoogleCredential googleCredential;
    //    private WalletobjectsService service;
    //    private readonly string issuerId;
    //    private readonly string classSuffix;
    //    private readonly string keyFilePath;
    //    private IHostingEnvironment _env;

    //    public GoogleWalletService3(IConfiguration configuration, IHostingEnvironment env)
    //    {
    //        _env = env;
    //        var walletSettings = configuration.GetSection("GoogleWalletSettings");

    //        //keyFilePath = Path.Combine(_env.w, "GoogleKeys", "natchibaech-9e515a4f00be.json");
    //        //string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "natchibaech-9e515a4f00be.json");
    //        //GoogleCredential credential = GoogleCredential.FromFile(keyPath)
    //        //    .CreateScoped(WalletobjectsService.ScopeConstants.WalletObjectIssuer);

    //        issuerId = walletSettings["IssuerId"];
    //        classSuffix = walletSettings["ClassId"];
    //        Auth();
    //    }

    //    private void Auth()
    //    {
    //        string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "GoogleKeys", "natchibaech-793937ea412e.json");
    //        GoogleCredential credential = GoogleCredential.FromFile(keyPath)
    //            .CreateScoped(WalletobjectsService.ScopeConstants.WalletObjectIssuer);

    //        //googleCredential = GoogleCredential
    //        //    .FromFile(keyFilePath)
    //        //    .CreateScoped(new[] {
    //        //    WalletobjectsService.ScopeConstants.WalletObjectIssuer
    //        //    });

    //        service = new WalletobjectsService(new BaseClientService.Initializer()
    //        {
    //            HttpClientInitializer = credential,
    //            ApplicationName = "Travel Site"
    //        });
    //    }

    //    private string CreateClass()
    //    {
    //        try
    //        {
    //            var existingClass = service.Eventticketclass
    //                .Get($"{issuerId}.{classSuffix}")
    //                .Execute();

    //            Console.WriteLine($"Class {existingClass.Id} already exists!");
    //            return existingClass.Id;
    //        }
    //        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
    //        {
    //            Console.WriteLine($"Class {issuerId}.{classSuffix} not found. Creating new class...");

    //            var newClass = new EventTicketClass
    //            {
    //                Id = $"{issuerId}.{classSuffix}",
    //                EventName = new LocalizedString
    //                {
    //                    DefaultValue = new TranslatedString
    //                    {
    //                        Language = "en-US",
    //                        Value = "Event Ticket"
    //                    }
    //                },
    //                IssuerName = "Travel Site",
    //                ReviewStatus = "UNDER_REVIEW",
    //                Venue = new EventVenue
    //                {
    //                    Name = new LocalizedString
    //                    {
    //                        DefaultValue = new TranslatedString
    //                        {
    //                            Language = "en-US",
    //                            Value = "Event Venue"
    //                        }
    //                    },
    //                    Address = new LocalizedString
    //                    {
    //                        DefaultValue = new TranslatedString
    //                        {
    //                            Language = "en-US",
    //                            Value = "123 Event St, City, Country"
    //                        }
    //                    }
    //                }
    //            };

    //            var insertedClass = service.Eventticketclass.Insert(newClass).Execute();
    //            Console.WriteLine("Class created successfully");
    //            return insertedClass.Id;
    //        }
    //    }

    //    public string CreateTicketWalletLink(Ticket ticket)
    //    {
    //        string classId = CreateClass();

    //        EventTicketObject ticketObject = new EventTicketObject
    //        {
    //            Id = $"{issuerId}.{ticket.TicketNumber}",
    //            ClassId = classId,
    //            State = "ACTIVE",
    //            TicketHolderName = ticket.MemberName,
    //            TicketNumber = ticket.TicketNumber,
    //            TicketType = new LocalizedString
    //            {
    //                DefaultValue = new TranslatedString
    //                {
    //                    Language = "en-US",
    //                    Value = "General Admission"
    //                }
    //            },
    //            ValidTimeInterval = new TimeInterval
    //            {
    //                Start = new Google.Apis.Walletobjects.v1.Data.DateTime
    //                {
    //                    Date = ticket.PurchaseDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
    //                },
    //                End = new Google.Apis.Walletobjects.v1.Data.DateTime
    //                {
    //                    Date = ticket.ExpiryDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
    //                }
    //            }
    //        };

    //        var insertedObject = service.Eventticketobject.Insert(ticketObject).Execute();

    //        return CreateJWTForTicket(insertedObject.Id, classId);
    //    }

    //    private string CreateJWTForTicket(string objectId, string classId)
    //    {
    //        JsonSerializerSettings excludeNulls = new JsonSerializerSettings()
    //        {
    //            NullValueHandling = NullValueHandling.Ignore
    //        };

    //        var objectsToAdd = new Dictionary<string, object>
    //    {
    //        {
    //            "eventTicketObjects", new List<EventTicketObject>
    //            {
    //                new EventTicketObject
    //                {
    //                    Id = objectId,
    //                    ClassId = classId
    //                }
    //            }
    //        }
    //    };

    //        JObject serializedPayload = JObject.Parse(
    //            JsonConvert.SerializeObject(objectsToAdd, excludeNulls));

    //        // Now safely cast googleCredential to ServiceAccountCredential
    //        var sac = googleCredential.UnderlyingCredential as ServiceAccountCredential;

    //        if (sac == null)
    //            throw new InvalidOperationException("Expected a ServiceAccountCredential");

    //        JObject jwtPayload = JObject.Parse(JsonConvert.SerializeObject(new
    //        {
    //            iss = sac.Id, // service account email
    //            aud = "google",
    //            origins = new[] { "www.example.com" }, // replace with your domain
    //            typ = "savetowallet",
    //            payload = serializedPayload
    //        }));

    //        JwtPayload claims = JwtPayload.Deserialize(jwtPayload.ToString());

    //        RsaSecurityKey key = new RsaSecurityKey(sac.Key);
    //        SigningCredentials signingCredentials = new SigningCredentials(
    //            key, SecurityAlgorithms.RsaSha256);

    //        JwtSecurityToken jwt = new JwtSecurityToken(
    //            new JwtHeader(signingCredentials), claims);

    //        string token = new JwtSecurityTokenHandler().WriteToken(jwt);

    //        string walletLink = $"https://pay.google.com/gp/v/save/{token}";
    //        Console.WriteLine("Add to Google Wallet link");
    //        Console.WriteLine(walletLink);

    //        return walletLink;
    //    }
    //}


    public interface IGoogleWalletService3
    {
        //public void Auth();
        //public string CreateClass();
        public string CreateTicketWalletLink(Ticket ticket);
    }

    #endregion

}
