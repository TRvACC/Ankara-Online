using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;

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

        }

        private async void SectorFilesView_Loaded(object sender, RoutedEventArgs e)
        {
            // these are only added for testing purposes, functions etc. will be added later
            euroscopeRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["EuroScopeRequiredVersion"] as string;
            sectorRequiredVersionSectorFilesText.Text = "2212-01"; // placeholder for now
            afvRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["AFVRequiredVersion"] as string;
            vatisRequiredVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["vATISRequiredVersion"] as string;

            euroscopeInstalledVersionSectorFilesText.Text = LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] as string;
            sectorInstalledVersionSectorFilesText.Text = "2212-01"; // placeholder for now
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
