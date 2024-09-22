using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SteamSpoofer.Windows;

namespace SteamSpoofer.Utility
{
    class Helper
    {
        public static string GetResource(string key)
        {
            return App.Current.Resources[key] as string ?? "";
        }
        
        public static void SetLogText(string text)
        {

        }
    }
}
