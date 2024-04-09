using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Email
{
    public class EmailConfigurationPuembo
    {
        public String? From { get; set; }
        public String? SmtpServer { get; set; }
        public int Port { get; set; }
        public String? UserName { get; set; }
        public String? Password { get; set; }
    }
}
