using System.Globalization;

namespace Fir.App.Services.Interfaces
{
    public interface IMailService
    {
        public Task Send(string from,string to,string subject,string text,string link,string name);
    }
}
