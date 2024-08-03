using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherXWinUI.Classes
{
    public static class DebugClass
    {
        public static void PrintList<T>(List<T> list)
        {
            foreach (var item in list)
            {
                Debug.WriteLine(item.ToString());
            }
        }
    }
}
