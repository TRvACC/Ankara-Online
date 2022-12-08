using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace Ankara_Online.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SectorFilesView : Page
    {
        /*
         * Example method for updating colors
         * E
        private void updateColors()
        {
            if (euroscopeInstalledVersionSectorFilesText.Text < euroscopeRequiredVersionSectorFilesText.Text || euroscopeInstalledVersionSectorFilesText.Text > euroscopeRequiredVersionSectorFilesText.Text)
            {
                euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
            }
            else
            {
                euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
            }
        }
        */

        public SectorFilesView()
        {
            this.InitializeComponent();

            // these are only added for testing purposes, functions etc. will be added later
            euroscopeRequiredVersionSectorFilesText.Text = "v3.2.1.26";
            sectorRequiredVersionSectorFilesText.Text = "2212-01";
            afvRequiredVersionSectorFilesText.Text = "1.10.1";
            vatisRequiredVersionSectorFilesText.Text = "v4.0.0.0-3";
            
            euroscopeInstalledVersionSectorFilesText.Text = "v3.2.1.29";
            sectorInstalledVersionSectorFilesText.Text = "2212-01";
            afvInstalledVersionSectorFilesText.Text = "1.10.1";
            vatisInstalledVersionSectorFilesText.Text = "v4.0.0.0-3";

            euroscopeInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Red);
            sectorInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
            afvInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
            vatisInstalledVersionSectorFilesText.Foreground = new SolidColorBrush(Colors.Green);
            
        }
    }
}
