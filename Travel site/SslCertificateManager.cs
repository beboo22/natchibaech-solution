using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Travelsite
{
    public static class SslCertificateManager
    {
        public static async Task<(string CertificatePfxPath, string CertificatePassword)> ObtainAndConfigureSslCertificate(
            string domain,
            string email,
            string outputPath,
            string certPassword,
            bool useStaging = true)
        {
            try
            {
                // Initialize ACME context with Let's Encrypt (staging or production)
                var acmeUri = useStaging ? WellKnownServers.LetsEncryptStagingV2 : WellKnownServers.LetsEncryptV2;
                var context = new AcmeContext(acmeUri);

                // Create or load account
                var accountKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
                var account = await context.NewAccount(new[] { $"mailto:{email}" }, true);

                // Save account key
                System.IO.Directory.CreateDirectory(outputPath);
                File.WriteAllText(Path.Combine(outputPath, "account-key.pem"), accountKey.ToPem());

                // Create a new order for the domain
                var order = await context.NewOrder(new[] { domain });

                // Get the HTTP-01 challenge
                var authz = (await order.Authorizations()).First();
                var httpChallenge = await authz.Http();
                var keyAuth = httpChallenge.KeyAuthz;

                // Prepare the challenge response file
                var challengePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), ".well-known", "acme-challenge");
                System.IO.Directory.CreateDirectory(challengePath);
                File.WriteAllText(Path.Combine(challengePath, httpChallenge.Token), keyAuth);

                // Validate the challenge (ensure accessible at http://<domain>/.well-known/acme-challenge/)
                await httpChallenge.Validate();

                // Wait for challenge validation
                for (int i = 0; i < 10; i++)
                {
                    var authzStatus = await authz.Resource();
                    if (authzStatus.Status == AuthorizationStatus.Valid)
                        break;
                    if (authzStatus.Status == AuthorizationStatus.Invalid)
                        throw new Exception("Challenge validation failed.");//here
                    await Task.Delay(3000);
                }

                // Generate certificate key and CSR
                var certKey = KeyFactory.NewKey(KeyAlgorithm.RS256);
                var cert = await order.Generate(
                    new CsrInfo
                    {
                        CountryName = "US",
                        State = "YourState",
                        Locality = "YourCity",
                        Organization = "YourOrganization",
                        CommonName = domain
                    }, certKey);

                // Save certificate and key as .pem files
                var certPemPath = Path.Combine(outputPath, "cert.pem");
                var keyPemPath = Path.Combine(outputPath, "cert-key.pem");
                File.WriteAllText(certPemPath, cert.ToPem());
                File.WriteAllText(keyPemPath, certKey.ToPem());

                // Convert to PFX using System.Security.Cryptography
                var certificate = X509Certificate2.CreateFromPemFile(certPemPath, keyPemPath);
                var exportBytes = certificate.Export(X509ContentType.Pfx, certPassword);

                var pfxPath = Path.Combine(outputPath, $"{domain}.pfx");
                File.WriteAllBytes(pfxPath, exportBytes);

                // Clean up
                System.IO.Directory.Delete(challengePath, true);
                File.Delete(certPemPath);
                File.Delete(keyPemPath);

                Console.WriteLine($"Certificate generated and saved to {pfxPath}");
                return (pfxPath, certPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obtaining certificate: {ex.Message}");
                throw;
            }
        }

        // Configure HTTPS in ASP.NET Core
        // Configure HTTPS in ASP.NET Core
        public static void ConfigureHttps(this WebApplicationBuilder hostBuilder, string certPath, string certPassword)
        {
            hostBuilder.WebHost.ConfigureKestrel(webBuilder =>
            {
                webBuilder.ListenAnyIP(443, listenOptions =>
                {
                    listenOptions.UseHttps(certPath, certPassword);
                });
                webBuilder.ListenAnyIP(80); // No need for HTTPS on port 80
            });
        }
    }

}
