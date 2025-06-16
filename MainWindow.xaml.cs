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
        public bool isPinging = true;
        public double roundTime = 0;
        public string url = "google.com";
        public int timeout = 1000;
        public string errorMessage = string.Empty;
        public int updateInterval = 10;
        public List<double> roundTimes = new List<double>();
        public int timeoutCount = 0;
        public int takeAvgFromLast = 10;

        public MainWindow()
        {
            InitializeComponent();
            Init();
            StartPingAsync();
        }

        public void Init()
        {
            Txb_Url.Text = url;
            Txb_Frequency.Text = updateInterval.ToString();
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

        public void RestartPinging()
        {
            isPinging = false;
            roundTimes.Clear();
            roundTime = 0;
            errorMessage = string.Empty;
            Txt_Ping.Text = "0";
            isPinging = true;
            StartPingAsync();
        }

        private void Txb_Url_LostFocus(object sender, RoutedEventArgs e)
        {
            Ping pingSender = new Ping();
            bool exceptionThrown = false;
            PingReply? reply = null;

            try
            {
                reply = pingSender.Send(Txb_Url.Text, timeout);
            }
            catch (Exception)
            {
                exceptionThrown = true;
                Txb_Url.Text = url;
            }

            if (!exceptionThrown && reply.Status == IPStatus.Success)
            {
                url = Txb_Url.Text;
                RestartPinging();
            }
            else
            {
                Txb_Url.Text = url;
            }
        }

        private void Txb_Frequency_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Txb_Frequency.Text.ToString().All(char.IsDigit) && !string.IsNullOrEmpty(Txb_Frequency.ToString()))
            {
                updateInterval = int.Parse(Txb_Frequency.Text.ToString());
                RestartPinging();
            }
            else
            {
                Txb_Frequency.Text = updateInterval.ToString();
            }
        }
    }
}