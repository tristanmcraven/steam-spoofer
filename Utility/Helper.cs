using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSpoofer.Utility
{
    class Helper
    {
        public static string GetResource(string key)
        {
            return App.Current.Resources[key] as string ?? "";
        }
    }
}
