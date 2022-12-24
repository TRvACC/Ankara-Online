using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using Newtonsoft.Json;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePageView : Page
    {
        public HomePageView()
        {
            this.InitializeComponent();

            euroscopeVersionHomeText.Text = LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] as string;
            sectorLTXXVersionHomeText.Text = LocalSettings.settingsContainer.Values["SectorFilesInstalledVersion"] as string;
            afvVersionHomeText.Text = LocalSettings.settingsContainer.Values["AFVInstalledVersion"] as string;
            vatisVersionHomeText.Text = LocalSettings.settingsContainer.Values["vATISInstalledVersion"] as string;

            this.Loading += HomePageView_Loading;
            this.Loaded += HomePageView_Loaded;
            reloadButton.Click += ReloadButton_Click;
            goOnlineButton.Click += GoOnlineButton_Click;

        }

        private void HomePageView_Loading(FrameworkElement sender, object args)
        {
            switch (Int32.Parse(LocalSettings.uiElementsDictionary["euroscopeVersionHomeText"]))
            {
                case -2:
                    euroscopeVersionHomeText.Text = "error";
                    euroscopeVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case -1:
                    euroscopeVersionHomeText.Text = " - ";
                    euroscopeVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case 0:
                    euroscopeVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case 1:
                    euroscopeVersionHomeText.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                case 2:
                    euroscopeVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
            }

            switch (Int32.Parse(LocalSettings.uiElementsDictionary["afvVersionHomeText"]))
            {
                case -1:
                    afvVersionHomeText.Text = " - ";
                    afvVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case 0:
                    afvVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case 1:
                    afvVersionHomeText.Foreground = new SolidColorBrush(Colors.Green);
                    break;
            }

            switch (Int32.Parse(LocalSettings.uiElementsDictionary["vatisVersionHomeText"]))
            {
                case -1:
                    vatisVersionHomeText.Text = " - ";
                    vatisVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case 0:
                    vatisVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case 1:
                    vatisVersionHomeText.Foreground = new SolidColorBrush(Colors.Green);
                    break;
            }

            HomePageViewICAO1.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO1"];
            HomePageViewICAO2.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO2"];
            HomePageViewICAO3.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO3"];

            HomePageViewICAO1_METAR.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"];
            HomePageViewICAO2_METAR.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO2_METAR"];
            HomePageViewICAO3_METAR.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO3_METAR"];

            HomePageViewICAO1_PROC.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"];
            HomePageViewICAO2_PROC.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"];
            HomePageViewICAO3_PROC.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"];

            if (HomePageViewICAO1_METAR.Text == "ERROR Fetching METAR" || HomePageViewICAO1_METAR.Text == "ERROR fetching LTFM METAR")
            {
                HomePageViewICAO1_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            if (HomePageViewICAO2_METAR.Text == "ERROR Fetching METAR" || HomePageViewICAO2_METAR.Text == "ERROR fetching LTFJ METAR")
            {
                HomePageViewICAO2_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            if (HomePageViewICAO3_METAR.Text == "ERROR Fetching METAR" || HomePageViewICAO3_METAR.Text == "ERROR fetching LTAI METAR")
            {
                HomePageViewICAO3_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            if (HomePageViewICAO1_PROC.Text == "LVP")
            {
                HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (HomePageViewICAO1_PROC.Text.EndsWith("Config"))
            {
                HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (HomePageViewICAO2_PROC.Text == "LVP")
            {
                HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (HomePageViewICAO2_PROC.Text.EndsWith("Config"))
            {
                HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (HomePageViewICAO3_PROC.Text == "LVP")
            {
                HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (HomePageViewICAO3_PROC.Text.EndsWith("Config"))
            {
                HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        private async void HomePageView_Loaded(object sender, RoutedEventArgs e)
        {
            if (LocalSettings.LTFM_METAR_PARSE_ERROR)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when parsing LTFM metar data. Please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is)",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
            else if (LocalSettings.LTFM_PRS_PARSE_ERROR)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when calculating PRS for LTFM. Please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }

            if (LocalSettings.LTFJ_METAR_PARSE_ERROR)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when parsing LTFJ metar data. Please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is)",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
            else if (LocalSettings.LTFJ_PRS_PARSE_ERROR)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when calculating PRS for LTFJ. Please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }

            if (LocalSettings.LTAI_METAR_PARSE_ERROR)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when parsing LTAI metar data. Please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is)",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
            else if (LocalSettings.LTAI_PRS_PARSE_ERROR)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when calculating PRS for LTAI. Please close the application and open a issue at https://github.com/cptalpdeniz/Ankara_Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
        }

        private void GoOnlineButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * CHECK VERSION AGAIN, IF ITS WRONG, DO NOT LET ATC TO USE WRONG PROFILE
             * POPUP ASKING FOR POSITION CALLSIGN LIKE LTFM_TWR
             * AFTER GETTING THE POSITION EDIT PRF FILE
             * OPEN EUROSCOPE WITH THE CORRECT PRF FILE
             */
            return;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            //reload data, call the reload function
            return;
        }
    }
}
