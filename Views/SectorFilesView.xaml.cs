using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.Drawing.Text;
using Windows.Storage.Pickers;
using System.Diagnostics;
using log4net.Appender;
using static System.Net.WebRequestMethods;
using System.IO;
using Microsoft.UI.Xaml.Shapes;
using Windows.System;
using Windows.ApplicationModel.Core;

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SectorFilesView : Page
    {
        public SectorFilesView()
        {
            this.InitializeComponent();

            this.Loaded += SectorFilesView_Loaded;

            downloadSectorFilesButton.Click += DownloadSectorFilesButton_Click;

            if (Controller.ControlIfSectorFilesInstalled())
            {
                downloadSectorFilesButton.IsEnabled = false;
            }
        }

        private async void DownloadSectorFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Controller.ControlIfSectorFilesInstalled()) 
            {
                Controller.InstallSectorFilesAsync();

                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Downloading",
                    Content = "Downloading sector files, please wait until you see the confirmation dialog",
                    CloseButtonText = "Close",
                };
                _ = await dialog.ShowAsync();

                bool fileExists = false;
                while(!fileExists)
                {
                    if (System.IO.File.Exists(Controller.gitSectorFilesPath + "\\version.txt"))
                    {
                        fileExists = true;
                        downloadSectorFilesButton.IsEnabled = false;
                        sectorInstalledVersionSectorFilesText.Text = "VALID";
                        sectorInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
                        ContentDialog dialog1 = new ContentDialog
                        {
                            XamlRoot = this.XamlRoot,
                            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                            Title = "Done",
                            Content = "Download completed",
                            CloseButtonText = "Close",
                        };
                        _ = await dialog1.ShowAsync();
                    }
                }

                /* Did not restart for some reason need to fix this */
                _ = CoreApplication.RequestRestartAsync("");

                Process.GetCurrentProcess().Kill();
            }
            else
            {
                downloadSectorFilesButton.IsEnabled = false;
            }
        }
        private async void SectorFilesView_Loaded(object sender, RoutedEventArgs e)
        {
            // these are only added for testing purposes, functions etc. will be added later
            euroscopeRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["EuroScopeRequiredVersion"] as string;
            sectorRequiredVersionSectorFilesText.Text = "LATEST"; // placeholder for now
            afvRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["AFVRequiredVersion"] as string;
            vatisRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["vATISRequiredVersion"] as string;

            euroscopeInstalledVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] as string;
            if (!Directory.Exists(Controller.gitSectorFilesPath + "\\LTXX"))
            {
                sectorInstalledVersionSectorFilesText.Text = "OUTDATED/NOT INSTALLED";
                sectorInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                downloadSectorFilesButton.IsEnabled = true;
            }
            else
            {
                sectorInstalledVersionSectorFilesText.Text = "VALID";
                sectorInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
            }
            
            afvInstalledVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["AFVInstalledVersion"] as string;
            vatisInstalledVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["vATISInstalledVersion"] as string;


            switch (Controller.EuroScopeVersionChecker())
            {
                case -2:
                    euroscopeInstalledVersionSectorFilesText.Text = "error";
                    euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadEuroscopeButton.IsEnabled = true;

                    break;
                case -1:
                    euroscopeInstalledVersionSectorFilesText.Text = " - ";
                    euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadEuroscopeButton.IsEnabled = true;
                    break;
                case 0:
                    euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadEuroscopeButton.IsEnabled = true;
                    break;
                case 1:
                    euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
                    downloadEuroscopeButton.IsEnabled = false;
                    break;
                case 2:
                    euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadEuroscopeButton.IsEnabled = true;
                    break;
            }

            switch (Controller.AFVVersionChecker())
            {
                case -1:
                    afvInstalledVersionSectorFilesText.Text = " - ";
                    afvInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadAFVButton.IsEnabled = true;
                    break;
                case 0:
                    afvInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadAFVButton.IsEnabled = true;
                    break;
                case 1:
                    afvInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
                    downloadAFVButton.IsEnabled = false;
                    break;
            }

            switch (await Controller.VATISVersionCheckerAsync())
            {
                case -1:
                    vatisInstalledVersionSectorFilesText.Text = " - ";
                    vatisInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadVATISButton.IsEnabled = true;
                    break;
                case 0:
                    vatisInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
                    downloadVATISButton.IsEnabled = true;
                    break;
                case 1:
                    vatisInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
                    downloadVATISButton.IsEnabled = false;
                    break;
            }
        }

    }
}
