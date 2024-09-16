using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamSpoofer.UserControls
{
    /// <summary>
    /// Interaction logic for CloseWindowButton.xaml
    /// </summary>
    public partial class CloseWindowButton : UserControl
    {
        public CloseWindowButton()
        {
            InitializeComponent();
        }

        private void closeWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
