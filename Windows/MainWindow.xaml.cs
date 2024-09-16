using SteamSpoofer.Windows;
using SteamSpoofer.Utility;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Interop;

namespace SteamSpoofer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            //StartProgress();
        }

        private void start_Button_Click(object sender, RoutedEventArgs e)
        {
            Spoofer.SpoofData();
        }

        private void titleBar_Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }




        
        //private void StartProgress()
        //{
        //    _timer = new DispatcherTimer();
        //    _timer.Interval = TimeSpan.FromSeconds(0.001); // Timer interval set to 100ms (10% per second)
        //    _timer.Tick += Timer_Tick;
        //    _timer.Start();
        //}

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    if (progressBar.Value < progressBar.Maximum)
        //    {
        //        progressBar.Value += 0.1; // Increase by 1% every tick (100ms)
        //    }
        //    else
        //    {
        //        _timer.Stop(); // Stop when maximum is reached
        //    }
        //}
    }
}