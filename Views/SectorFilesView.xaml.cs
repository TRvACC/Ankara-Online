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

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SectorFilesView : Page
    {
        bool gitWarning = false;
        public SectorFilesView()
        {
            this.InitializeComponent();

            this.Loaded += SectorFilesView_Loaded;

            downloadSectorFilesButton.Click += DownloadSectorFilesButton_Click;
        }

        private async void DownloadSectorFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Controller.ControlIfSFInstalled() == 0) 
            {
                string path = System.IO.Path.Combine(Controller.gitSfPath.Remove(40), "sector-files");
                System.IO.Directory.CreateDirectory(path);


                Process batch;
                batch = Process.Start(@"C:\Users\raven\source\repos\Ankara-Online\downloadSF.bat");
                int i = 0;
                while(i == 0)
                {
                    if (batch.HasExited)
                    {
                        i = 1;
                        downloadSectorFilesButton.IsEnabled = false;
                        ContentDialog dialog = new ContentDialog
                        {
                            XamlRoot = this.XamlRoot,
                            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                            Title = "Done",
                            Content = "Download Completed",
                            CloseButtonText = "Close",
                        };
                        _ = await dialog.ShowAsync();
                    }
                }
                
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
            sectorRequiredVersionSectorFilesText.Text = "2305-01"; // placeholder for now
            afvRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["AFVRequiredVersion"] as string;
            vatisRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["vATISRequiredVersion"] as string;

            euroscopeInstalledVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] as string;
            sectorInstalledVersionSectorFilesText.Text = "2305-01"; // placeholder for now
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
