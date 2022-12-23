using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using WinUIEx;

namespace Ankara_Online
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen : WinUIEx.SplashScreen
    {
        public SplashScreen(Type window) : base(window)
        {
            this.InitializeComponent();
            this.CenterOnScreen();
            this.splashScreenImage1.Source = new BitmapImage(new Uri("ms-appx:///Assets/TRvACC/trvacc_icon_transparent.png"));
            this.splashScreenImage2.Source = new BitmapImage(new Uri("ms-appx:///Assets/TRvACC/trvacc_noicon_animated.gif"));
            versionText.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        protected override async Task OnLoading()
        {
            if (!LocalSettings.settingsContainer.Containers.ContainsKey("VATSIM_ID"))
            {
                loadingTextBlock.Text = $"Loading 0%...";
                App.log.Info("Local Settings does not exists. Creating settings for VATSIM ID, Hoppie LOGON Code, App Version, Required and Installed paths.");
                LocalSettings.settingsContainer.Values["VATSIM_ID"] = null;
                LocalSettings.settingsContainer.Values["HoppieLOGONCode"] = null;
                loadingTextBlock.Text = $"Loading 5%...";
                LocalSettings.settingsContainer.Values["AppVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                loadingTextBlock.Text = $"Loading 8%...";
                LocalSettings.settingsContainer.Values["EuroScopePath"] = Controller.GetEuroScopePath();
                loadingTextBlock.Text = $"Loading 12%...";
                LocalSettings.settingsContainer.Values["SectorFilesPath"] = null;
                loadingTextBlock.Text = $"Loading 15%...";
                LocalSettings.settingsContainer.Values["vATISPath"] = Controller.GetVATISPath();
                loadingTextBlock.Text = $"Loading 20%...";
                LocalSettings.settingsContainer.Values["AFVPath"] = Controller.GetAFVPath();
                loadingTextBlock.Text = $"Loading 27%...";

                LocalSettings.settingsContainer.Values["EuroScopeRequiredVersion"] = Controller.GetEuroScopeRequiredVersion();
                loadingTextBlock.Text = $"Loading 31%...";
                LocalSettings.settingsContainer.Values["SectorFilesRequiredVersion"] = null;
                loadingTextBlock.Text = $"Loading 37%...";
                LocalSettings.settingsContainer.Values["AFVRequiredVersion"] = Controller.GetAFVRequiredVersion();
                loadingTextBlock.Text = $"Loading 42%...";
                LocalSettings.settingsContainer.Values["vATISRequiredVersion"] = await Controller.GetVATISRequiredVersionAsync();
                loadingTextBlock.Text = $"Loading 57%...";

                LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] = Controller.GetEuroScopeInstalledVersion();
                loadingTextBlock.Text = $"Loading 61%...";
                LocalSettings.settingsContainer.Values["SectorFilesInstalledVersion"] = null;
                loadingTextBlock.Text = $"Loading 69%...";
                LocalSettings.settingsContainer.Values["AFVInstalledVersion"] = Controller.GetAFVInstalledVersion();
                loadingTextBlock.Text = $"Loading 92%...";
                LocalSettings.settingsContainer.Values["vATISInstalledVersion"] = Controller.GetVATISInstalledVersion();
                loadingTextBlock.Text = $"Loading 99%...";

                LocalSettings.settingsContainer.Values["AFV_VERSION_CHECK_URL"] = "https://github.com/vatsimnetwork/afv-clients/blob/main/clientversion.xml";
                LocalSettings.settingsContainer.Values["vATIS_VERSION_CHECK_JSON"] = "https://vatis.clowd.io/api/v4/VersionCheck";
                LocalSettings.settingsContainer.Values["TRvACC_SMART_API"] = "https://smart.trvacc.net/api";
                loadingTextBlock.Text = $"Loading 100%...";
            }

            //HomePageView.UpdateMetarTextData();

        }
    }
}
