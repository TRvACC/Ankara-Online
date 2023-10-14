using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;
using WinUIEx;
using log4net;
using System.Net.Http;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Net.NetworkInformation;

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
            if (CheckInternetConnection())
            {
                if (!LocalSettings.settingsContainer.Containers.ContainsKey("VATSIM_ID"))
                {
                    loadingTextBlock.Text = $"Loading 0%...";
                    App.log.Info("Local Settings does not exists. Creating settings for VATSIM ID, Hoppie LOGON Code, App Version, Required and Installed paths.");
                    LocalSettings.settingsContainer.Values["VATSIM_ID"] = string.Empty;
                    LocalSettings.settingsContainer.Values["HoppieLOGONCode"] = string.Empty;
                    loadingTextBlock.Text = $"Loading 5%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["AppVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    await Task.Delay(50);

                    loadingTextBlock.Text = $"Loading 6%...";
                    LocalSettings.settingsContainer.Values["EuroScopePath"] = Controller.GetEuroScopePath();
                    loadingTextBlock.Text = $"Loading 7%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["SectorFilesPath"] = string.Empty;
                    loadingTextBlock.Text = $"Loading 8%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["vATISPath"] = Controller.GetVATISPath();
                    loadingTextBlock.Text = $"Loading 10%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["AFVPath"] = Controller.GetAFVPath();
                    loadingTextBlock.Text = $"Loading 14%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["EuroScopeRequiredVersion"] = Controller.GetEuroScopeRequiredVersion();
                    loadingTextBlock.Text = $"Loading 15%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["SectorFilesRequiredVersion"] = null;
                    loadingTextBlock.Text = $"Loading 18%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["AFVRequiredVersion"] = Controller.GetAFVRequiredVersion();
                    loadingTextBlock.Text = $"Loading 22%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["vATISRequiredVersion"] = await Controller.GetVATISRequiredVersionAsync();
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] = Controller.GetEuroScopeInstalledVersion();
                    loadingTextBlock.Text = $"Loading 23%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["SectorFilesInstalledVersion"] = null;
                    loadingTextBlock.Text = $"Loading 28%...";
                    await Task.Delay(50);
                    LocalSettings.settingsContainer.Values["AFVInstalledVersion"] = Controller.GetAFVInstalledVersion();

                    LocalSettings.settingsContainer.Values["vATISInstalledVersion"] = Controller.GetVATISInstalledVersion();

                    LocalSettings.settingsContainer.Values["AFV_VERSION_CHECK_URL"] = "https://github.com/vatsimnetwork/afv-clients/blob/main/clientversion.xml";
                    LocalSettings.settingsContainer.Values["vATIS_VERSION_CHECK_JSON"] = "https://vatis.clowd.io/api/v4/VersionCheck";
                    LocalSettings.settingsContainer.Values["TRvACC_METAR_API"] = "https://rasat.trvacc.net/metar/";

                }
            }
            else
            {
                await Task.Delay(900);
                loadingTextBlock.Text = $"ERROR! This program requires active internet connection. Without internet connection it cannot function.";
                await Task.Delay(5000);
                Process.GetCurrentProcess().Kill();
            }

            LocalSettings.uiElementsDictionary["euroscopeVersionHomeText"] = Controller.EuroScopeVersionChecker().ToString();
            LocalSettings.uiElementsDictionary["afvVersionHomeText"] = Controller.AFVVersionChecker().ToString();
            LocalSettings.uiElementsDictionary["vatisVersionHomeText"] = (await Controller.VATISVersionCheckerAsync()).ToString();

            loadingTextBlock.Text = $"Loading 31%...";
            await Task.Delay(50);

            LocalSettings.uiElementsDictionary["HomePageViewICAO1"] = "LTFM";
            LocalSettings.uiElementsDictionary["HomePageViewICAO2"] = "LTFJ";
            LocalSettings.uiElementsDictionary["HomePageViewICAO3"] = "LTAI";

            string[] badWeatherCategory = { "GR", "GS", "IC", "PL", "PO", "RA", "SN", "SA", "SG", "SS", "TS", "UP", "VA" };

            string ICAO1_METAR = await GetMetarJSONAsync("LTFM");
            string ICAO2_METAR = await GetMetarJSONAsync("LTFJ");
            string ICAO3_METAR = await GetMetarJSONAsync("LTAI");

            dynamic icao1MetarObj = null;
            dynamic icao2MetarObj = null;
            dynamic icao3MetarObj = null;

            var ltfmMetarFetchSuccess = false;
            var ltfjMetarFetchSuccess = false;
            var ltaiMetarFetchSuccess = false;

            // JSON deserialization for LTFM
            try
            {
                App.log.Info("Deserializing LTFM metar JSON into object");
                icao1MetarObj = JsonConvert.DeserializeObject<dynamic>(ICAO1_METAR);
                App.log.Info("Deserialization of LTFM metar JSON into object completed successfully");
                LocalSettings.LTFM_METAR_PARSE_ERROR = false;
                ltfmMetarFetchSuccess = true;
            }
            catch (Exception e)
            {
                LocalSettings.LTFM_METAR_PARSE_ERROR = true;

                App.log.Error("Could not deserialize LTFM metar JSON data. The metar string is: \n" + ICAO1_METAR + "\nException " + e.ToString() + " occurred.");

                icao1MetarObj = null;
                ltfmMetarFetchSuccess = false;
                LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"] = "ERROR Fetching METAR";
            }
            loadingTextBlock.Text = $"Loading 32%...";
            await Task.Delay(50);

            // JSON deserialization for LTFJ
            try
            {
                App.log.Info("Deserializing LTFJ metar JSON into object");
                icao2MetarObj = JsonConvert.DeserializeObject<dynamic>(ICAO2_METAR);
                App.log.Info("Deserialization of LTFJ metar JSON into object completed successfully");
                LocalSettings.LTFJ_METAR_PARSE_ERROR = false;
                ltfjMetarFetchSuccess = true;
            }
            catch (Exception e)
            {
                LocalSettings.LTFJ_METAR_PARSE_ERROR = true;
                App.log.Error("Could not deserialize LTFJ metar JSON data. The metar string is: \n" + ICAO2_METAR + "\nException " + e.ToString() + " occurred.");

                icao2MetarObj = null;
                ltfjMetarFetchSuccess = false;

                LocalSettings.uiElementsDictionary["HomePageViewICAO2_METAR"] = "ERROR Fetching METAR";
            }

            loadingTextBlock.Text = $"Loading 34%...";
            await Task.Delay(50);

            if (Controller.ControlIfSectorFilesInstalled())
            {
                Controller.UpdateSectorFilesCMD();
            }
             
            // JSON deserialization for LTAI
            try
            {
                App.log.Info("Deserializing LTAI metar JSON into object");
                icao3MetarObj = JsonConvert.DeserializeObject<dynamic>(ICAO3_METAR);
                App.log.Info("Deserialization of LTAI metar JSON into object completed successfully");
                LocalSettings.LTAI_METAR_PARSE_ERROR = false;
                ltaiMetarFetchSuccess = true;
            }
            catch (Exception e)
            {
                LocalSettings.LTAI_METAR_PARSE_ERROR = true;
                App.log.Error("Could not deserialize LTAI metar JSON data. The metar string is: \n" + ICAO3_METAR + "\nException " + e.ToString() + " occurred.");

                icao3MetarObj = null;
                ltaiMetarFetchSuccess = false;

                LocalSettings.uiElementsDictionary["HomePageViewICAO3_METAR"] = "ERROR Fetching METAR";
            }

            loadingTextBlock.Text = $"Loading 36%...";
            await Task.Delay(50);

            /*
            // Parse LTFM PRS
            if (ICAO1_METAR != null && icao1MetarObj != null && ltfmMetarFetchSuccess)
            {
                try
                {
                    App.log.Info("Starting to calculate PRS for LTFM");
                    if (Int32.Parse(icao1MetarObj["visibility"].ToString()) <= 550 || (icao1MetarObj.cloud[0].ContainsKey("cloud_base_ft_agl") && Int32.Parse(icao1MetarObj["cloud"][0]["cloud_base_ft_agl"].ToString()) <= 200))
                    {
                        LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"] += " - ";
                        LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " LVP";
                    }
                    else
                    {
                        // check if weather category exists
                        if (icao1MetarObj.ContainsKey("weather"))
                        {
                            // check if weather is terrible by matching category
                            foreach (string i in badWeatherCategory)
                            {
                                if (icao1MetarObj["weather"].ToString().Substring(icao1MetarObj["weather"].ToString().Length - 2) == i)
                                {
                                    // if wind speed is greater or equal then 5 knots, then PRS applies
                                    if (Int32.Parse(icao1MetarObj["wind_speed"].ToString()) >= 5)
                                    {
                                        var windDirection = Int32.Parse(icao1MetarObj["wind_direction"].ToString());
                                        if (windDirection <= 84 || windDirection >= 264)
                                        {
                                            LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " North Config";
                                        }
                                        else
                                        {
                                            LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " South Config";
                                        }
                                    }
                                    // if wind speed is less than 5 knots then north config
                                    else
                                    {
                                        LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " North Config";
                                    }
                                }
                            }
                        }
                        else
                        {
                            var windDirection = Int32.Parse(icao1MetarObj["wind_direction"].ToString());
                            if (Int32.Parse(icao1MetarObj["wind_speed"].ToString()) >= 10)
                            {
                                if (windDirection >= 264 || windDirection <= 84)
                                {
                                    LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " North Config";
                                }
                                else
                                {
                                    LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " South Config";
                                }
                            }
                            else
                            {
                                LocalSettings.uiElementsDictionary["HomePageViewICAO1_PROC"] = " North Config";
                            }
                        }
                    }

                    LocalSettings.LTFM_PRS_PARSE_ERROR = false;
                    App.log.Info("PRS Calculation for LTFM finished without error.");
                    LocalSettings.uiElementsDictionary["HomePageViewICAO1"] += " -   ";
                }
                catch (Exception e)
                {
                    if (LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"] != "ERROR Fetching METAR")
                    {
                        LocalSettings.LTFM_PRS_PARSE_ERROR = false;
                    }

                    App.log.Error("LTFM PRS calculation failed. Exception: " + e.ToString());
                }
                LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"] = icao1MetarObj["metar"].ToString().Substring(icao1MetarObj["metar"].ToString().IndexOf(' ') + 1);
            }
            else if (ICAO1_METAR == null)
            {
                LocalSettings.uiElementsDictionary["HomePageViewICAO1_METAR"] = "ERROR fetching LTFM METAR";
            }

            loadingTextBlock.Text = $"Loading 54%...";
            await Task.Delay(50);
            // PRS calculation for LTFJ
            if (ICAO2_METAR != null && icao2MetarObj != null && ltfjMetarFetchSuccess)
            {
                try
                {
                    App.log.Info("Starting to calculate PRS for LTFJ");
                    if (Int32.Parse(icao2MetarObj["visibility"].ToString()) <= 400 || (icao2MetarObj.cloud[0].ContainsKey("cloud_base_ft_agl") && Int32.Parse(icao2MetarObj["cloud"][0]["cloud_base_ft_agl"].ToString()) <= 200))
                    {
                        LocalSettings.uiElementsDictionary["HomePageViewICAO2"] += " - ";
                        LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " LVP";
                    }
                    else
                    {
                        // check if weather header exists
                        if (icao2MetarObj.ContainsKey("weather"))
                        {
                            // check if weather is terrible by matching category
                            foreach (string i in badWeatherCategory)
                            {
                                if (icao2MetarObj["weather"].ToString().Substring(icao2MetarObj["weather"].ToString().Length - 2) == i)
                                {
                                    // if wind speed is greater or equal then 5 knots, then PRS applies
                                    if (Int32.Parse(icao2MetarObj["wind_speed"].ToString()) >= 5)
                                    {
                                        var windDirection = Int32.Parse(icao2MetarObj["wind_direction"].ToString());
                                        if (windDirection >= 329 || windDirection <= 149)
                                        {
                                            LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " North Config";
                                        }
                                        else
                                        {
                                            LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " South Config";
                                        }
                                    }
                                    // if wind speed is less than 5 knots then north config
                                    else
                                    {
                                        LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " North Config";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Int32.Parse(icao2MetarObj["wind_speed"].ToString()) >= 10)
                            {
                                var windDirection = Int32.Parse(icao2MetarObj["wind_direction"].ToString());
                                if (windDirection >= 329 || windDirection <= 149)
                                {
                                    LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " North Config";
                                }
                                else
                                {
                                    LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " South Config";
                                }
                            }
                            // if wind speed is less than 10 knots then north config
                            else
                            {
                                LocalSettings.uiElementsDictionary["HomePageViewICAO2_PROC"] = " North Config";
                            }
                        }

                    }

                    App.log.Info("PRS Calculation for LTFJ finished without error.");
                    LocalSettings.uiElementsDictionary["HomePageViewICAO2"] += " -   ";

                    LocalSettings.LTFJ_PRS_PARSE_ERROR = false;
                }
                catch (Exception e)
                {
                    if (LocalSettings.uiElementsDictionary["HomePageViewICAO2_METAR"] != "ERROR Fetching METAR")
                    {
                        LocalSettings.LTFJ_PRS_PARSE_ERROR= true;
                    }

                    App.log.Error("LTFM PRS calculation failed. Exception: " + e.ToString());
                }

                LocalSettings.uiElementsDictionary["HomePageViewICAO2_METAR"] = icao2MetarObj["metar"].ToString().Substring(icao2MetarObj["metar"].ToString().IndexOf(' ') + 1);
            }
            else if (ICAO2_METAR == null)
            {
                LocalSettings.uiElementsDictionary["HomePageViewICAO2_METAR"] = "ERROR fetching LTFJ METAR";
            }

            loadingTextBlock.Text = $"Loading 65%...";
            await Task.Delay(50);

            // PRS calculation for LTAI
            if (ICAO3_METAR != null && icao3MetarObj != null && ltaiMetarFetchSuccess)
            {
                try
                {
                    App.log.Info("Starting to calculate PRS for LTAI");

                    if (Int32.Parse(icao3MetarObj["visibility"].ToString()) <= 550 || (icao3MetarObj.cloud[0].ContainsKey("cloud_base_ft_agl") && Int32.Parse(icao3MetarObj["cloud"][0]["cloud_base_ft_agl"].ToString()) <= 200))
                    {
                        LocalSettings.uiElementsDictionary["HomePageViewICAO3"] += " - ";
                        LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " LVP";
                    }
                    else
                    {
                        // check if weather header exists
                        if (icao3MetarObj.ContainsKey("weather"))
                        {
                            // check if weather is terrible by matching category
                            foreach (string i in badWeatherCategory)
                            {
                                if (icao3MetarObj["weather"].ToString().Substring(icao3MetarObj["weather"].ToString().Length - 2) == i)
                                {
                                    // if wind speed is greater or equal then 5 knots, then PRS applies
                                    if (Int32.Parse(icao3MetarObj["wind_speed"].Value) >= 5)
                                    {
                                        var windDirection = Int32.Parse(icao3MetarObj["wind_direction"].ToString());
                                        if (windDirection >= 271 || windDirection <= 91)
                                        {
                                            LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " North Config";
                                        }
                                        else
                                        {
                                            LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " South Config";
                                        }
                                    }
                                    // if wind is less than 5 knots
                                    else
                                    {
                                        LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " North Config";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((Int32.Parse(icao3MetarObj["wind_speed"].ToString())) >= 10)
                            {
                                var windDirection = Int32.Parse(icao3MetarObj["wind_direction"].ToString());
                                if (windDirection >= 271 || windDirection <= 91)
                                {
                                    LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " North Config";

                                }
                                else
                                {
                                    LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " South Config";
                                }
                            }
                            else
                            {
                                LocalSettings.uiElementsDictionary["HomePageViewICAO3_PROC"] = " North Config";
                            }
                        }

                    }

                    App.log.Info("PRS Calculation for LTAI finished without error.");
                    LocalSettings.uiElementsDictionary["HomePageViewICAO3"] += " -   ";

                    LocalSettings.LTAI_PRS_PARSE_ERROR = false;
                }
                catch (Exception e)
                {
                    if (LocalSettings.uiElementsDictionary["HomePageViewICAO3_METAR"] != "ERROR Fetching METAR")
                    {
                        LocalSettings.LTAI_PRS_PARSE_ERROR = true;
                    }

                    App.log.Error("LTAI PRS calculation failed. Exception: " + e.ToString());
                }

                LocalSettings.uiElementsDictionary["HomePageViewICAO3_METAR"] = icao3MetarObj["metar"].ToString().Substring(icao3MetarObj["metar"].ToString().IndexOf(' ') + 1);
            }
            else if (ICAO3_METAR == null)
            {
                LocalSettings.uiElementsDictionary["HomePageViewICAO3_METAR"] = "ERROR fetching LTAI METAR";
            }
            */

            if (Controller.ControlIfSectorFilesInstalled())
            {
                Controller.ReadProfiles();
            }

            loadingTextBlock.Text = $"Loading 100%...";
        }

        private bool CheckInternetConnection()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "rasat.trvacc.net";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                App.log.Error("Ping to rasat.trvacc.net failed. Without internet connection this program cannot work. The issue is either due to RASAT server error or user's computer not connected to the internet (or connection being blocked)" + e.ToString());
                return false;
            }
        }

        internal static async Task<string> GetMetarJSONAsync(string ICAO)
        {
            string metarJSON = null;

            using HttpClient client = new HttpClient();

            // get metar of the passed ICAO// 
            try
            {
                metarJSON = await client.GetStringAsync(LocalSettings.settingsContainer.Values["TRvACC_METAR_API"] as string + ICAO + "/json");
            }
            catch (Exception e)
            {
                App.log.Error("Error when trying to fetching METAR of " + ICAO + " . \n" + e.ToString());
            }
            //"\n<div 
            if (metarJSON != null && metarJSON.StartsWith("\"\\n<div "))
            {
                return null;
            }
            return metarJSON;
        }
    }
}
