using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config.KTheme
{
    public class KTThemeBase
    {
        public string Name { get; set; } = "";

        public string LayoutDir { get; set; } = "";

        public string Direction { get; set; } = "";

        public bool ModeSwitchEnabled { get; set; } = false;

        public string ModeDefault { get; set; } = "";

        public string AssetsDir { get; set; } = "";

        public string IconsType { get; set; } = "";

        public KTThemeAssets Assets { get; set; } = new KTThemeAssets();

        public SortedDictionary<string, SortedDictionary<string, string[]>> Vendors { get; set; } = new SortedDictionary<string, SortedDictionary<string, string[]>>();
    }

}
