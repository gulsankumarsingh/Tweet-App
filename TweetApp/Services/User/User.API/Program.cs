namespace User.API
{
    using Azure.Identity;
    using Common.Logging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Defines the <see cref="Program" />.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The Main.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// The CreateHostBuilder.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog(SeriLogger.Configure)
            .ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var builtConfig = configBuilder.Build();

                if (hostingContext.HostingEnvironment.IsProduction())
                {
                    configBuilder.AddAzureKeyVault(new Uri(builtConfig["AzureKeyVault:Vault"]), new DefaultAzureCredential());
                }

                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    using var x509Store = new X509Store(StoreLocation.CurrentUser);

                    x509Store.Open(OpenFlags.ReadOnly);

                    var certificate = x509Store.Certificates
                        .Find(
                            X509FindType.FindByThumbprint,
                            builtConfig["AzureKeyVault:Thumbprint"],
                            validOnly: false)
                        .OfType<X509Certificate2>()
                        .Single();

                    configBuilder.AddAzureKeyVault(new Uri(builtConfig["AzureKeyVault:Vault"]), new ClientCertificateCredential(
                    builtConfig["AzureKeyVault:TenentId"],
                       builtConfig["AzureKeyVault:ClientId"],
                       certificate));

                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    }
}
