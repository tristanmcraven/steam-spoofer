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
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.status_TextBlock.Text = GetResource(text);
            });
        }
        public static void SetLogText(string text, string appendix)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.status_TextBlock.Text = GetResource(text) + $" {appendix}";
            });
        }
    }
}
