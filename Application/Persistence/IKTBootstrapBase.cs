using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IKTBootstrapBase
    {
        void InitThemeMode();

        void InitThemeDirection();

        void InitLayout();

        void Init(IKTTheme theme);
    }
}
