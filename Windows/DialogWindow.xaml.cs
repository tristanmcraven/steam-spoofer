using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SteamSpoofer.Windows
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public bool Result = false;
        public DialogWindow()
        {
            InitializeComponent();
        }

        public DialogWindow(string title, string message)
        {
            InitializeComponent();
            SystemSounds.Exclamation.Play();
            title_TextBlock.Text = title;
            message_TextBlock.Text = message;
            Application.Current.Windows.OfType<MainWindow>().First().blackTint.Visibility = Visibility.Visible;
        }

        private void confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }

        private void cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Windows.OfType<MainWindow>().First().blackTint.Visibility = Visibility.Collapsed;
        }
    }
}
