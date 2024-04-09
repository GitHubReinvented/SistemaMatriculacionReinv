using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config.KTheme
{
    public class KTThemeSettings
    {
        public static KTThemeBase Config { get; set; } = new KTThemeBase();

        public static void init(IConfiguration configuration)
        {
            Config = configuration.GetSection("Theme").Get<KTThemeBase>() ?? Config;
        }
    }
}
