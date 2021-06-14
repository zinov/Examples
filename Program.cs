using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AzureKeyVaultExample;
using Microsoft.Extensions.Configuration;

namespace KeyVault
{
    //public class Program
    //{
    //    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
    //        .SetBasePath(Directory.GetCurrentDirectory())
    //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false)
    //        .AddEnvironmentVariables()
    //        .Build();

    //    public static void Main(string[] args)
    //    {
    //        CreateHostBuilder(args).Build().Run();
    //    }

    //    public static IHostBuilder CreateHostBuilder(string[] args) =>
    //        Host.CreateDefaultBuilder(args)
    //            .ConfigureAppConfiguration((context, config) =>
    //            {
    //                var thumb = Configuration["KeyVault:ThumbPrint"];
    //                var clientid = Configuration["KeyVault:ClientId"];
    //                var Vaultname = Configuration["KeyVault:VaultName"];
    //                if (context.HostingEnvironment.IsDevelopment())
    //                {
    //                    //using (var store = new X509Store(System.Security.Cryptography.X509Certificates.StoreName.Root))
    //                    using (var store = new X509Store(StoreLocation.CurrentUser))
    //                    {
    //                        store.Open(OpenFlags.ReadOnly);
    //                        var certs = store.Certificates
    //                            .Find(X509FindType.FindByThumbprint, thumb, false);

    //                        config.AddAzureKeyVault($"https://" + Vaultname + ".vault.azure.net/", clientid, certs.OfType<X509Certificate2>().Single());

    //                        store.Close();
    //                    }
    //                }
    //            })
    //            .ConfigureWebHostDefaults(webBuilder =>
    //            {
    //                webBuilder.UseStartup<Startup>();
    //            });
    //}

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    var configuration = builder.Build();
                    var clientid = configuration["keyvault:ClientId"];
                    var vaultUrl = configuration["keyvault:VaultUrl"];
                    //Add KeyVault to the configuration providers
                    builder.AddAzureKeyVault(vaultUrl, clientid, LoadCertificateFromStore(configuration),
                        new PrefixSecretManager("AzureKeyVaultExample"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static X509Certificate2 LoadCertificateFromStore(IConfiguration configuration)
        {
            string thumbPrint = configuration["KeyVault:ThumbPrint"];

            //passing the personal store location
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,
                    thumbPrint, false);
                if (certCollection.Count == 0)
                {
                    throw new Exception("The specified certificate wasn't found.");
                }

                return certCollection[0];
            }
        }
    }
}
