using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SteamSpoofer.Utility
{
    class WindowManager
    {
        public static Window GetWindow<TWindow>() where TWindow : Window
        {
            return Application.Current.Windows.OfType<TWindow>().First()! as TWindow;
        }
    }
}
