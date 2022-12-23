using Microsoft.UI.Xaml;
using log4net;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
namespace Ankara_Online
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            _isNetworkOnline = false;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            log.Info("Starting application"); //log4net
            
            var splash = new SplashScreen(typeof(MainWindow));
            splash.Completed += (s, e) => m_window = (WindowEx)e;

            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        // Not sure about this behavior but keep it in mind for now
        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            _isNetworkOnline = NetworkInterface.GetIsNetworkAvailable();
            if (!(_isNetworkOnline))
            {
                log.Error("Connection Lost! This program requires internet connection to run.");
            }
        }

        // this function will probably move to Splash Scren when implemented
        private async Task<bool> CheckIfNetworkConnectionAsync()
        {
            log.Debug("Checking if there is any active network connection");
            bool networkConnection = NetworkInterface.GetIsNetworkAvailable();
            bool isAPIConnectionAchieved = false;
            string APIreturn;

            if (networkConnection)
            {
                try
                {
                    log.Info("Establish API connection to check connection status");
                    using HttpClient client = new HttpClient();

                    APIreturn = await client.GetStringAsync("https://smart.trvacc.net/api/");
                    dynamic apiReturnObj = JsonConvert.DeserializeObject<dynamic>(APIreturn);

                    if (apiReturnObj != null && apiReturnObj["status"].ToString() == "success")
                    {
                        log.Info("API connection established.");
                        isAPIConnectionAchieved = true;
                    }
                    else
                    {
                        log.Info("API connection was not established.");
                        throw new Exception("Either the API connection was not established or received faulty response from server");
                    }
                }
                catch
                {
#pragma warning disable IDE0090
                    ContentDialog dialog = new ContentDialog
                    {
                        XamlRoot = this.m_window.Content.XamlRoot,
                        Style = Current.Resources["DefaultContentDialogStyle"] as Style,
                        Title = "Error!",
                        Content = "API connection failed. If you think this was a mistake, re-launch the App. If the issue persists, please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                        CloseButtonText = "OK",
                    };
#pragma warning restore IDE0090

                    _ = await dialog.ShowAsync();

                    isAPIConnectionAchieved = false;
                    log.Error("API connection was not established. The program cannot run properly without API connection.");
                    Environment.Exit(0);
                }
            }
            else
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.m_window.Content.XamlRoot,
                    Style = Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "No internet connection found. This application does not work without an active internet application. Please check your internet connection and then try again. If you have active internet connection but still getting network error, please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();

                log.Error("No network connection found!");
            }

            log.Info("Network connection is available and program was able to successfully connect to TRvACC SMART API");

            return networkConnection;
        }

        internal static async Task<string> GetMetarJSONAsync(string ICAO)
        {
            string metarJSON = null;

            using HttpClient client = new HttpClient();

            // get metar of // 
            try
            {
                metarJSON = await client.GetStringAsync("https://smart.trvacc.net/api/metar/" + ICAO);
            }
            catch (Exception e)
            {
                log.Error("Error when trying to fetching METAR of " + ICAO + " . \n" + e.ToString());
            }
            //"\n<div 
            if (metarJSON != null && metarJSON.StartsWith("\"\\n<div "))
            {
                return null;
            }
            return metarJSON;
        }

        private Window m_window;
        internal static readonly ILog log = LogManager.GetLogger(typeof(App));
        private bool _isNetworkOnline;
        
    }
}
