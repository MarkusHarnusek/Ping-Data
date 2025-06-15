using System.Text;
using System.Windows;
using System.Net.NetworkInformation;

namespace PingData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool isPinging = true;
        public static double roundTime = 0;
        public static string url = "google.com";
        public static int timeout = 1000;
        public static string errorMessage = string.Empty;
        public static int updateInterval = 10;
        public static List<double> roundTimes = new List<double>();
        public static int timeoutCount = 0;
        public static int takeAvgFromLast = 10;

        public MainWindow()
        {
            InitializeComponent();
            StartPingAsync();
        }

        public async void StartPingAsync()
        {
            Ping pingSender = new Ping();

            while (isPinging)
            {
                PingReply reply = await pingSender.SendPingAsync(url, timeout);

                Dispatcher.Invoke(() =>
                { 
                    if (reply.Status == IPStatus.Success)
                    {
                        roundTime = reply.RoundtripTime;
                        errorMessage = string.Empty;
                        roundTimes.Add(roundTime);

                        if (roundTimes.Count > takeAvgFromLast)
                        {
                            Txt_Ping.Text = ((int)roundTimes.TakeLast(takeAvgFromLast).Average()).ToString();
                        }
                        else
                        {
                            Txt_Ping.Text = ((int)roundTimes.Average()).ToString();
                        }
                    }
                    else
                    {
                        roundTime = 0;
                        errorMessage = reply.Status.ToString();
                    }
                });

                await Task.Delay(updateInterval);
            }
        }

        private void RestartPinging()
        {
            isPinging = false;
            roundTimes.Clear();
            roundTime = 0;
            errorMessage = string.Empty;
            Txt_Ping.Text = "0";
            isPinging = true;
            StartPingAsync();
        }
    }
}