using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Sentry;

namespace fantasy_hoops
{
    public class Program
    {
        private static KeyVaultClient _keyVaultClient;
        public const string KEY_VAULT_ENDPOINT = "https://fantasy-hoops.vault.azure.net";


        public static KeyVaultClient GetKeyVaultClient()
        {
            if (_keyVaultClient == null)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider(
                    "RunAs=App;AppId=9c1be78d-8e41-4887-9be3-1b54eaefb2d1;TenantId=4c6b4065-35b7-4671-b6fe-2f7ca805942b;AppKey=3-Eh7n8[e6TQ:W.-d=RLG-[@PYAMdDnq"
                );
                _keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
            }

            return _keyVaultClient;
        }
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder.AddAzureKeyVault(
                        KEY_VAULT_ENDPOINT, GetKeyVaultClient(), new DefaultKeyVaultSecretManager());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSentry();
                });
    }
}