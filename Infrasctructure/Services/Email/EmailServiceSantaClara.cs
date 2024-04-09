
using Application.Models.Email;
using Application.Persistence;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasctructure.Services.Email
{
    public class EmailServiceSantaClara : IEmailServiceSantaClara
    {
        private readonly EmailConfigurationSantaClara _emailConfiguration;
        private readonly ITemplateService _templateService;
        private readonly IWebHostEnvironment _environment;

        public EmailServiceSantaClara(
            EmailConfigurationSantaClara emailConfiguration,
            ITemplateService templateService,
            IWebHostEnvironment environment
            )
        {
            _emailConfiguration = emailConfiguration;
            _templateService = templateService;
            _environment = environment;
        }

        public string GeneratePassword()
        {
            Random rdn = new Random();
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 6;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }

            return contraseniaAleatoria;
        }

        public async Task SendEmailHtml(string to, string subject, object model, string templateName)
        {
            try
            {
                var headerImagePath = string.Format("{0}/{1}", _environment.WebRootPath, "assets/media/Reinvented/Logo1.png");

                var email = new EmailData();

                // email.HeaderImage = new LinkedResource
                // {
                //     ContentId = "header",
                //     ContentPath = headerImagePath,
                //     ContentType = "image/png"
                // };
                email.HtmlContent = await GetGridContentAsync(model, templateName);

                string fromAddress = _emailConfiguration.From!;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromAddress, fromAddress));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;


                // create a new instance of the BodyBuilder class
                var builder = new BodyBuilder();


                if (email.HeaderImage != null)
                {
                    // add a header image linked resource
                    // to the builder
                    var header = builder.LinkedResources.Add(
                        email.HeaderImage.ContentPath);

                    // set the contentId for the resource
                    // added to the builder to the contentId passed
                    header.ContentId = email.HeaderImage.ContentId;
                }

                if (email.Attachment != null)
                {
                    var attachment = email.Attachment;

                    // add the LinkedResource data
                    // to the Attachments property 
                    // on the builder

                    // pass the ContentId,
                    // Content ByteArray
                    // and the ContentType
                    builder.Attachments.Add(
                        attachment.ContentId,
                        attachment.ContentBytes,
                        ContentType.Parse(attachment.ContentType));
                }

                // set the html body (since we are passing html content to render)
                // to the body of the builder
                builder.HtmlBody = email.HtmlContent;

                // convert all the configuration made
                // into a message content which 
                // is assigned to the message body
                message.Body = builder.ToMessageBody();

                // send the prepared message
                await SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task<string> GetGridContentAsync(object data, string templateName)
        {
            var templatePath = templateName;
            return await _templateService.GetTemplateHtmlAsStringAsync(templatePath, data);
        }


        //Envia el mensaje
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
                    await client.SendAsync(mailMessage);
                    await client.DisconnectAsync(true);
                }
                catch(Exception ex) 
                {
                    //log an error message or throw an exception, or both.
                    throw ex;
                }

            }
        }

    }
}
