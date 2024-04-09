using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config.KTheme
{
    public class KTIconsSettings
    {
        public static SortedDictionary<string, int> Config { get; set; } = new SortedDictionary<string, int>();

        public static void init(IConfiguration configuration)
        {
            Config = configuration.GetSection("duotone-paths").Get<SortedDictionary<string, int>>() ?? Config;
        }
    }
}
