using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



namespace mvc
{
    public class Program
    {
        public static SslProtocols Protocol;
        public static void Main(string[] args)
        {
            if (args.Count() < 4)
            {
                System.Console.WriteLine("Required: <CertificatePath> <CertificatePassword> <HTTPPortNumber> <HTTPSPortNumber> <SslProtocol>");
                Environment.Exit(1);
            }
            if (args.Count() == 5)
            {
                if (Enum.TryParse(typeof(SslProtocols), args[4], true, out object o))
                {
                    Protocol = (SslProtocols)o;
                }
                else
                {
                    var names = String.Join(", ", Enum.GetNames(typeof(SslProtocols)).OrderBy(x => x).ToArray());
                    System.Console.WriteLine("{0} is an invalid ssl protocol. Valid values are {1}", args[4], names);
                    Environment.Exit(1);
                }
            }
            else
            {
                Protocol = SslProtocols.Tls12;
            }
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>().UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, int.Parse(args[2]));
                    options.Listen(IPAddress.Loopback, int.Parse(args[3]), listenOptions =>
                    {
                        var certificate = new X509Certificate2(args[0], args[1]);
                        HttpsConnectionAdapterOptions httpsOption = new HttpsConnectionAdapterOptions();
                        httpsOption.SslProtocols = Protocol;
                        httpsOption.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                        httpsOption.ClientCertificateValidation = (inCertificate, inChain, inPolicy) => { return true; };
                        httpsOption.CheckCertificateRevocation = false;
                        httpsOption.ServerCertificate = certificate;
                        listenOptions.UseHttps(httpsOption);
                    });
                })
                .Build();
    }
}
