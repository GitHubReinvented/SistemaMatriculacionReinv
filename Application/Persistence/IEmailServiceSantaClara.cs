using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IEmailServiceSantaClara
    {
        Task SendEmailHtml(string to, string subject, object model, string templateName);
        string GeneratePassword();
    }
}
