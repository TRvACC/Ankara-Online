using Microsoft.UI.Xaml;
using log4net;
using log4net.Config;

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
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            log.Info("Starting application"); //log4net
            //Splash screen icin burasi
            //m_window = new SplashScreenView();
            m_window = new MainWindow();
            m_window.Activate();

        }

        private Window m_window;
        internal static readonly ILog log = LogManager.GetLogger(typeof(App));
    }
}
