using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using Newtonsoft.Json;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using log4net;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;
using System.IO;
using Windows.Devices.Bluetooth.Background;
using Windows.ApplicationModel.Background;
using System.Security.AccessControl;
using System.Diagnostics;

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePageView : Page
    {
        private bool ICAO1metarHasRun = false;
        private bool ICAO2metarHasRun = false;
        private bool ICAO3metarHasRun = false;
        private bool ICAO1PRSHasRun = false;
        private bool ICAO2PRSHasRun = false;
        private bool ICAO3PRSHasRun = false;
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

            inputTextBox = new TextBox
            {
                AcceptsReturn = true,
                Height = 32,
                Width = 150,
                Text = string.Empty,
                TextWrapping = TextWrapping.Wrap,
            };

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

            // HomePageViewICAO1_METAR.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"];
            // HomePageViewICAO2_METAR.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO2_METAR"];
            // HomePageViewICAO3_METAR.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO3_METAR"];

            // HomePageViewICAO1_PROC.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"];
            // HomePageViewICAO2_PROC.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"];
            // HomePageViewICAO3_PROC.Text = LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"];

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
            if (!Directory.Exists(Controller.gitSectorFilesPath + "\\LTXX"))
            {
                sectorLTXXVersionHomeText.Text = "OUTDATED/NOT INSTALLED";
                sectorLTXXVersionHomeText.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                sectorLTXXVersionHomeText.Text = "VALID";
                sectorLTXXVersionHomeText.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (LocalSettings.LTFM_METAR_PARSE_ERROR && !ICAO1metarHasRun)
            {
                ICAO1metarHasRun = true;
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when parsing LTFM metar data. Please close the application and open a issue at https://github.com/TRvACC/Ankara-Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is)",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
            else if (LocalSettings.LTFM_PRS_PARSE_ERROR && !ICAO1PRSHasRun)
            {
                ICAO1PRSHasRun = true;
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when calculating PRS for LTFM. Please close the application and open a issue at https://github.com/TRvACC/Ankara-Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }

            if (LocalSettings.LTFJ_METAR_PARSE_ERROR && !ICAO2metarHasRun)
            {
                ICAO1metarHasRun = true;
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
            else if (LocalSettings.LTFJ_PRS_PARSE_ERROR && !ICAO2PRSHasRun)
            {
                ICAO2PRSHasRun = true;
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when calculating PRS for LTFJ. Please close the application and open a issue at https://github.com/TRvACC/Ankara-Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }

            if (LocalSettings.LTAI_METAR_PARSE_ERROR && !ICAO3metarHasRun)
            {
                ICAO3metarHasRun = true;
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when parsing LTAI metar data. Please close the application and open a issue at https://github.com/TRvACC/Ankara-Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is)",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
            else if (LocalSettings.LTAI_PRS_PARSE_ERROR && !ICAO3PRSHasRun)
            {
                ICAO3PRSHasRun = true;
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when calculating PRS for LTAI. Please close the application and open a issue at https://github.com/TRvACC/Ankara-Online/issues and upload ALL the Ankara_Online.log files (located where Ankara_Online.exe is). Write in detail what you were doing with steps and when did the error happened.",
                    CloseButtonText = "OK",
                };
#pragma warning restore IDE0090

                _ = await dialog.ShowAsync();
            }
        }

        public async void GoOnlineButton_Click(object sender, RoutedEventArgs e)
        {
            Controller.UpdateSectorFilesCMD();
            /*
             * CHECK VERSION AGAIN, IF ITS WRONG, DO NOT LET ATC TO USE WRONG PROFILE
             * POPUP ASKING FOR POSITION CALLSIGN LIKE LTFM_TWR
             * AFTER GETTING THE POSITION EDIT PRF FILE
             * OPEN EUROSCOPE WITH THE CORRECT PRF FILE
             */

            // LocalSettings.CorrectSectorFilesVersion should be added when the Sector Files checker is implemented
            if (LocalSettings.correctEuroScopeVersion && LocalSettings.correctAFVVersion && LocalSettings.correctVATISVersion && LocalSettings.settingsContainer.Values["VATSIM_IDactual"] != null && LocalSettings.settingsContainer.Values["UserRealName"] != null && LocalSettings.settingsContainer.Values["VATSIM_Password"] != null && LocalSettings.settingsContainer.Values["HoppieLOGONCodeActual"] != null)
            {
#pragma warning disable IDE0090
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Please select a callsign!",
                    Content = inputTextBox,
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "Select",

                };
                dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

                _ = await dialog.ShowAsync();
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Required applications are either not installed or the installed versions are not correct. Please go to \"Applications and Sector Files\" page and install or update your software.",
                    PrimaryButtonText = "OK",
                };
#pragma warning restore IDE0090
                dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

                _ = await dialog.ShowAsync();

            }
        }

        private async void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            string prfPath = null;
            /*
            CHECK IF THE CALLSIGN IS AVAILABLE
            EDIT THE PRF FILE
            AND OPEN EUROSCOPE WITH CORRECT PRF FILE
            */

            if (inputTextBox.Text == string.Empty)
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "You need to inpute correct callsign of the sector position, e.g. LTFM_TWR",
                    CloseButtonText = "OK",
                };
                _ = await dialog.ShowAsync();
            }
            else
            {
                foreach (Profile temp in LocalSettings.profileList)
                {
                    if (inputTextBox.Text == temp.positionName)
                    {
                        string tempPostionEnding = temp.positionName.Substring(temp.positionName.Length - 3);

                        switch (tempPostionEnding)
                        {
                            case "CTR":
                                {
                                    prfPath = Controller.gitSectorFilesPath + "\\ACC-APP.prf";
                                    break;
                                }
                            case "APP":
                                {
                                    prfPath = Controller.gitSectorFilesPath + "\\ACC-APP.prf";
                                    break;
                                }
                            case "TWR":
                                {
                                    prfPath = Controller.gitSectorFilesPath + "\\GRP MODE-S.prf";
                                    break;
                                }
                            case "GND":
                                {
                                    prfPath = Controller.gitSectorFilesPath + "\\GRP MODE-S.prf";
                                    break;
                                }
                            case "DEL":
                                {
                                    prfPath = Controller.gitSectorFilesPath + "\\GRP MODE-S.prf";
                                    break;
                                }
                        }

                        if (prfPath != null)
                        {
                            File.AppendAllText(prfPath,
                                                Environment.NewLine + "LastSession\tcallsign\t" + temp.positionName + Environment.NewLine);


                            File.AppendAllText(prfPath,
                                                "LastSession\trealname\t" + LocalSettings.settingsContainer.Values["UserRealName"] + Environment.NewLine);

                            File.AppendAllText(prfPath,
                                                "LastSession\tcertificate\t" + LocalSettings.settingsContainer.Values["VATSIM_IDactual"] + Environment.NewLine);
                            
                            File.AppendAllText(prfPath,
                                                "LastSession\tpassword\t" + LocalSettings.settingsContainer.Values["VATSIM_Password"] + Environment.NewLine);

                            File.AppendAllText(prfPath,
                                                "LastSession\tatis2\t" + temp.ATIS2 + Environment.NewLine);

                            File.AppendAllText(prfPath,
                                                "LastSession\tatis3\t" + temp.ATIS3 + Environment.NewLine);

                            File.AppendAllText(prfPath,
                                                "LastSession\tatis4\t" + temp.ATIS4 + Environment.NewLine);
                            File.AppendAllText(prfPath,
                                                "LastSession\tfacility\t" + (int)temp.facility + Environment.NewLine);

                            File.AppendAllText(prfPath,
                                                "LastSession\tserver\t" + "AUTOMATIC" + Environment.NewLine);

                            File.AppendAllText(prfPath,
                                                "LastSession\trange\t" + temp.range + Environment.NewLine);

                            string euroscopePath = LocalSettings.settingsContainer.Values["EuroScopePath"].ToString();

                            Process process = new Process();
                            process.StartInfo.FileName = euroscopePath + "\\EuroScope.exe";
                            process.StartInfo.Arguments = prfPath;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();

                        }
                    }
                }
            }            
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            //reload data, call the reload function
            return;
        }

        private TextBox inputTextBox;
    }
}
