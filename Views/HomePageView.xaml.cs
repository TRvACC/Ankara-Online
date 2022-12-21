using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
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

            reloadButton.Click += ReloadButton_Click;
            goOnlineButton.Click += GoOnlineButton_Click;
            UpdateMetarTextData();
        }

        internal async void UpdateMetarTextData()
        {
            HomePageViewICAO1.Text = "LTFM";
            HomePageViewICAO2.Text = "LTFJ";
            HomePageViewICAO3.Text = "LTAI";

            string ICAO1_METAR = await App.GetMetarJSONAsync(HomePageViewICAO1.Text);
            string ICAO2_METAR = await App.GetMetarJSONAsync(HomePageViewICAO2.Text);
            string ICAO3_METAR = await App.GetMetarJSONAsync(HomePageViewICAO3.Text);

            dynamic icao1MetarObj = null;
            dynamic icao2MetarObj = null;
            dynamic icao3MetarObj = null;

            // JSON deserialization for LTFM
            try
            {
                App.log.Info("Deserializing LTFM metar JSON into object");
                icao1MetarObj = JsonConvert.DeserializeObject<dynamic>(ICAO1_METAR);
                App.log.Info("Deserialization of LTFM metar JSON into object completed successfully");
            }
            catch (Exception e)
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

                App.log.Error("Could not deserialize LTFM metar JSON data. The metar string is: \n" + ICAO1_METAR + "\nException " + e.ToString() + " occurred.");

                icao1MetarObj = null;

                HomePageViewICAO1_METAR.Text = "ERROR Fetching METAR";
                HomePageViewICAO1_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            // JSON deserialization for LTFJ
            try
            {
                App.log.Info("Deserializing LTFJ metar JSON into object");
                icao2MetarObj = JsonConvert.DeserializeObject<dynamic>(ICAO2_METAR);
                App.log.Info("Deserialization of LTFJ metar JSON into object completed successfully");

            }
            catch (Exception e)
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

                App.log.Error("Could not deserialize LTFJ metar JSON data. The metar string is: \n" + ICAO2_METAR + "\nException " + e.ToString() + " occurred.");

                icao2MetarObj = null;

                HomePageViewICAO2_METAR.Text = "ERROR Fetching METAR";
                HomePageViewICAO2_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            // JSON deserialization for LTAI
            try
            {
                App.log.Info("Deserializing LTAI metar JSON into object");
                icao3MetarObj = JsonConvert.DeserializeObject<dynamic>(ICAO3_METAR);
                App.log.Info("Deserialization of LTAI metar JSON into object completed successfully");
            }
            catch (Exception e)
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

                App.log.Error("Could not deserialize LTAI metar JSON data. The metar string is: \n" + ICAO3_METAR + "\nException " + e.ToString() + " occurred.");

                icao3MetarObj = null;

                HomePageViewICAO3_METAR.Text = "ERROR Fetching METAR";
                HomePageViewICAO3_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            // Parse LTFM metar
            if (ICAO1_METAR != null && icao1MetarObj != null && !(HomePageViewICAO1_METAR.Text.StartsWith("ERROR Fetching METAR")))
            {
                try
                {
                    App.log.Info("Starting to calculate PRS for LTFM");
                    if (Int32.Parse(icao1MetarObj["visibility"].ToString()) <= 550 || (icao1MetarObj.cloud[0].ContainsKey("cloud_base_ft_agl") && Int32.Parse(icao1MetarObj["cloud"][0]["cloud_base_ft_agl"].ToString()) <= 200))
                    {
                        HomePageViewICAO1.Text += " - ";
                        HomePageViewICAO1_PROC.Text = " LVP";
                        HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        // check if weather category exists
                        if (icao1MetarObj.ContainsKey("weather"))
                        {
                            // check if weather is terrible by matching category
                            foreach (string i in App.badWeatherCategory)
                            {
                                if (icao1MetarObj["weather"].ToString().Substring(icao1MetarObj["weather"].ToString().Length - 2) == i)
                                {
                                    // if wind speed is greater or equal then 5 knots, then PRS applies
                                    if (Int32.Parse(icao1MetarObj["wind_speed"].ToString()) >= 5)
                                    {
                                        var windDirection = Int32.Parse(icao1MetarObj["wind_direction"].ToString());
                                        if (windDirection <= 84 || windDirection >= 264)
                                        {
                                            HomePageViewICAO1_PROC.Text = " North Config";
                                            HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                        }
                                        else
                                        {
                                            HomePageViewICAO1_PROC.Text = " South Config";
                                            HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                        }
                                    }
                                    // if wind speed is less than 5 knots then north config
                                    else
                                    {
                                        HomePageViewICAO1_PROC.Text = " North Config";
                                        HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
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
                                    HomePageViewICAO1_PROC.Text = " North Config";
                                    HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                }
                                else
                                {
                                    HomePageViewICAO1_PROC.Text = " South Config";
                                    HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                }
                            }
                            else
                            {
                                HomePageViewICAO1_PROC.Text = " North Config";
                                HomePageViewICAO1_PROC.Foreground = new SolidColorBrush(Colors.Green);
                            }
                        }

                    }

                    App.log.Info("PRS Calculation for LTFM finished without error.");
                    HomePageViewICAO1.Text += " -   ";
                }
                catch (Exception e)
                {
                    if (HomePageViewICAO1_METAR.Text != "ERROR Fetching METAR")
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

                    App.log.Error("LTFM PRS calculation failed. Exception: " + e.ToString());
                }
                HomePageViewICAO1_METAR.Text = icao1MetarObj["metar"].ToString().Substring(icao1MetarObj["metar"].ToString().IndexOf(' ') + 1);
            }
            else if (ICAO1_METAR == null)
            {
                HomePageViewICAO1_METAR.Text = "Error fetching " + HomePageViewICAO1.Text + " METAR";
                HomePageViewICAO1_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            // PRS calculation for LTFJ
            if (ICAO2_METAR != null && icao2MetarObj != null && !(HomePageViewICAO2_METAR.Text.StartsWith("ERROR Fetching METAR")))
            {
                try
                {
                    App.log.Info("Starting to calculate PRS for LTFJ");
                    if (Int32.Parse(icao2MetarObj["visibility"].ToString()) <= 550 || (icao2MetarObj.cloud[0].ContainsKey("cloud_base_ft_agl") && Int32.Parse(icao2MetarObj["cloud"][0]["cloud_base_ft_agl"].ToString()) <= 200))
                    {
                        HomePageViewICAO2.Text += " - ";
                        HomePageViewICAO2_PROC.Text = " LVP";
                        HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        // check if weather header exists
                        if (icao2MetarObj.ContainsKey("weather"))
                        {
                            // check if weather is terrible by matching category
                            foreach (string i in App.badWeatherCategory)
                            {
                                if (icao2MetarObj["weather"].ToString().Substring(icao2MetarObj["weather"].ToString().Length - 2) == i)
                                {
                                    // if wind speed is greater or equal then 5 knots, then PRS applies
                                    if (Int32.Parse(icao2MetarObj["wind_speed"].ToString()) >= 5)
                                    {
                                        var windDirection = Int32.Parse(icao2MetarObj["wind_direction"].ToString());
                                        if (windDirection >= 329 || windDirection <= 149)
                                        {
                                            HomePageViewICAO2_PROC.Text = " North Config";
                                            HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                        }
                                        else
                                        {
                                            HomePageViewICAO2_PROC.Text = " South Config";
                                            HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                        }
                                    }
                                    // if wind speed is less than 5 knots then north config
                                    else
                                    {
                                        HomePageViewICAO2_PROC.Text = " North Config";
                                        HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
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
                                    HomePageViewICAO2_PROC.Text = " North Config";
                                    HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                }
                                else
                                {
                                    HomePageViewICAO2_PROC.Text = " South Config";
                                    HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                }
                            }
                            // if wind speed is less than 10 knots then north config
                            else
                            {
                                HomePageViewICAO2_PROC.Text = " North Config";
                                HomePageViewICAO2_PROC.Foreground = new SolidColorBrush(Colors.Green);
                            }
                        }

                    }

                    App.log.Info("PRS Calculation for LTFJ finished without error.");

                    HomePageViewICAO2.Text += " -   ";
                }
                catch (Exception e)
                {
                    if (HomePageViewICAO2_METAR.Text != "ERROR Fetching METAR")
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
                    App.log.Error("LTFM PRS calculation failed. Exception: " + e.ToString());
                }

                HomePageViewICAO2_METAR.Text = icao2MetarObj["metar"].ToString().Substring(icao2MetarObj["metar"].ToString().IndexOf(' ') + 1);
            }
            else if (ICAO2_METAR == null)
            {
                HomePageViewICAO2_METAR.Text = "Error fetching " + HomePageViewICAO2.Text + "METAR";
                HomePageViewICAO2_METAR.Foreground = new SolidColorBrush(Colors.Red);
            }

            // PRS calculation for LTAI
            if (ICAO3_METAR != null && icao3MetarObj != null && !(HomePageViewICAO3_METAR.Text.StartsWith("ERROR Fetching METAR")))
            {
                try
                {
                    App.log.Info("Starting to calculate PRS for LTAI");

                    if (Int32.Parse(icao3MetarObj["visibility"].ToString()) <= 550 || (icao3MetarObj.cloud[0].ContainsKey("cloud_base_ft_agl") && Int32.Parse(icao3MetarObj["cloud"][0]["cloud_base_ft_agl"].ToString()) <= 200))
                    {
                        HomePageViewICAO3.Text += " - ";
                        HomePageViewICAO3_PROC.Text = " LVP";
                        HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        // check if weather header exists
                        if (icao3MetarObj.ContainsKey("weather"))
                        {
                            // check if weather is terrible by matching category
                            foreach (string i in App.badWeatherCategory)
                            {
                                if (icao3MetarObj["weather"].ToString().Substring(icao3MetarObj["weather"].ToString().Length - 2) == i)
                                {
                                    // if wind speed is greater or equal then 5 knots, then PRS applies
                                    if (Int32.Parse(icao3MetarObj["wind_speed"].Value) >= 5)
                                    {
                                        var windDirection = Int32.Parse(icao3MetarObj["wind_direction"].ToString());
                                        if (windDirection >= 271 || windDirection <= 91)
                                        {
                                            HomePageViewICAO3_PROC.Text = " North Config";
                                            HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);

                                        }
                                        else
                                        {
                                            HomePageViewICAO3_PROC.Text = " South Config";
                                            HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                        }
                                    }
                                    // if wind is less than 5 knots
                                    else
                                    {
                                        HomePageViewICAO3_PROC.Text = " North Config";
                                        HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);
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
                                    HomePageViewICAO3_PROC.Text = " North Config";
                                    HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                    
                                }
                                else
                                {
                                    HomePageViewICAO3_PROC.Text = " South Config";
                                    HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);
                                }
                            }
                            else
                            {
                                HomePageViewICAO3_PROC.Text = " North Config";
                                HomePageViewICAO3_PROC.Foreground = new SolidColorBrush(Colors.Green);
                            }
                        }

                    }

                    App.log.Info("PRS Calculation for LTAI finished without error.");

                    HomePageViewICAO3.Text += " -   ";
                }
                catch (Exception e)
                {
                    if (HomePageViewICAO3_METAR.Text != "ERROR Fetching METAR")
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

                    App.log.Error("LTAI PRS calculation failed. Exception: " + e.ToString());
                }

                HomePageViewICAO3_METAR.Text = icao3MetarObj["metar"].ToString().Substring(icao3MetarObj["metar"].ToString().IndexOf(' ') + 1);
            }
            else if (ICAO3_METAR == null)
            {
                HomePageViewICAO3_METAR.Text = "Error fetching " + HomePageViewICAO3.Text + "METAR";
                HomePageViewICAO3_METAR.Foreground = new SolidColorBrush(Colors.Red);
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
