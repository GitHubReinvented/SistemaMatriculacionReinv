using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config.KTheme
{
    public class KTThemeAssets
    {
        public string Favicon { get; set; } = "";

        public List<string> Fonts { get; set; } = new List<string>();

        public List<string> Css { get; set; } = new List<string>();

        public List<string> Js { get; set; } = new List<string>();
    }
}
