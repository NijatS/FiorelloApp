using Fir.App.Services.Interfaces;
using Fir.Core.Entities;
using System.Net.Mail;

namespace Fir.App.Services.Implementations
{
    public class MailService : IMailService
    {
        private readonly IWebHostEnvironment _env;

        public MailService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task Send(string from, string to, string subject,string text, string link, string name)
        {
            string body = string.Empty;
            string path = Path.Combine(_env.WebRootPath, "assets", "Templates", "email.html");
            using (StreamReader SourceReader = System.IO.File.OpenText(path))
            {
                body = SourceReader.ReadToEnd();
            }
            body = body.Replace("{{Link}}", link);
            body = body.Replace("{{Name}}", name);
            body = body.Replace("{{Text}}", text);
            MailMessage mm = new MailMessage();
            mm.To.Add(to);
            mm.Subject = subject;
            mm.Body = body;
            mm.IsBodyHtml = true;
            mm.From = new MailAddress(from);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(from, "gmaagjxgczxovsrw");

            await smtp.SendMailAsync(mm);
        }
    }
}
