using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace EmailTemplateBuilder
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }


        static void Main(string[] args)
        {
            var sendProductionEmail = false; // Change it to true to use secrets.json and send real emails.

            var path = Path.Combine(Environment.CurrentDirectory, @"Templates\dist\sample.html");
            var recipients = new[] { "emailtemplatebuildertest@mailinator.com" };
            var subject = $"Email Template Builder Test ({DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})";

            var settings = InitializeAndGetSettings();

            using (var client = InitializeAndGetSmtpClient(!sendProductionEmail, settings))
            {
                Console.WriteLine("Building email with the following settings:\n" +
                    $"--Server: {(!sendProductionEmail ? "127.0.0.1" : settings.Server)}\n" +
                    $"--Recipient(s): {string.Join(", ", recipients)}\n" +
                    $"--Subject: {subject}\n" +
                    $"--Template: {path.Replace(Environment.CurrentDirectory, "")}");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(settings.SenderEmail),
                    Body = LoadEmailTemplate(path),
                    Subject = subject,
                    IsBodyHtml = true
                };

                foreach (var recipient in recipients)
                {
                    mailMessage.To.Add(recipient);
                }

                Console.WriteLine("Sending email...");

                client.Send(mailMessage);

                Console.WriteLine("Email successfully sent.");
            }

            Console.ReadKey();
        }

        private static Settings InitializeAndGetSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            // This shouldn't be added in non-development mode. If it is used in test then further development is
            // required.
            builder.AddUserSecrets<Settings>();

            Configuration = builder.Build();

            var services = new ServiceCollection()
               .Configure<Settings>(Configuration.GetSection(nameof(Settings)))
               .AddOptions()
               .BuildServiceProvider();

            var settings = services.GetService<IOptions<Settings>>();

            return settings.Value;
        }

        private static SmtpClient InitializeAndGetSmtpClient(bool localhost, Settings settings)
        {
            var client = new SmtpClient(localhost ? "127.0.0.1" : settings.Server)
            {
                UseDefaultCredentials = false
            };

            if (!localhost)
            {
                client.Credentials = new NetworkCredential(settings.Username, settings.Password);
                client.Port = settings.Port;
                if (settings.Secure)
                {
                    client.EnableSsl = true;
                }
            }

            return client;
        }

        private static string LoadEmailTemplate(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
