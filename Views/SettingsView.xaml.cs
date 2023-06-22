using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Text.RegularExpressions;
using Windows.System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Reflection;
using System.Drawing;
using System.IO;
using Windows.ApplicationModel.Search;
using Windows.Storage.AccessCache;
using System.ComponentModel.DataAnnotations;

namespace Ankara_Online
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();

            settingsCreditsTextBlock.Text = "Credits\nAlp Deniz Senyurt - ACCTR5\nCan Bartu Topcuoglu\nVersion: " + localSettings.Values["AppVersion"] as string;

            settingsVatsimCIDBox.Text = LocalSettings.settingsContainer.Values["VATSIM_ID"] as string;
            settingsHoppieRichEditBox.Text = LocalSettings.settingsContainer.Values["HoppieLOGONCode"] as string;
            settingsESPathTextBox.Text = LocalSettings.settingsContainer.Values["EuroScopePath"] as string;
            settingsAFVPathTextBox.Text = LocalSettings.settingsContainer.Values["AFVPath"] as string;
            settingsVATISPathTextBox.Text = LocalSettings.settingsContainer.Values["vATISPath"] as string;

            this.UpdateColors();
            // register events
            settingsVatsimCIDBox.TextChanging += SettingsIDEditBox_TextChanging;
            settingsVatsimCIDBox.Paste += SettingsIDEditBox_Paste;
            getHoppieLOGONCodeButton.Click += GetHoppieLOGONCodeButton_Click;
            settingsPageSaveButton.Click += SettingsPageSaveButton_Click;
            settingsESPathSelectButton.Click += SettingsESPathSelectButton_Click;
            settingsAFVPathSelectButton.Click += SettingsAFVPathSelectButton_Click;
            settingsVATISPathSelectButton.Click += SettingsVATISPathSelectButton_Click;

            settingsPageSaveButton.Click += SettingsPageSaveButton_Click1;
            this.Loaded += SettingsView_Loaded;
        }
        private void SettingsPageSaveButton_Click1(object sender, RoutedEventArgs e)
        {
            LocalSettings.settingsContainer.Values["VATSIM_IDactual"] = settingsVatsimCIDBox.Text;
            LocalSettings.settingsContainer.Values["UserRealName"] = settingsNameBox.Text;
            LocalSettings.settingsContainer.Values["VATSIM_Password"] = settingsPasswordBox.Password;
            LocalSettings.settingsContainer.Values["HoppieLOGONCodeActual"] = settingsHoppieRichEditBox.Text;
        }

        private void SettingsView_Loaded(object sender, RoutedEventArgs e)
        {
            settingsVatsimCIDBox.Text = LocalSettings.settingsContainer.Values["VATSIM_IDactual"] as string;
            settingsNameBox.Text = LocalSettings.settingsContainer.Values["UserRealName"] as string;
            settingsPasswordBox.Password = LocalSettings.settingsContainer.Values["VATSIM_Password"] as string;
            settingsHoppieRichEditBox.Text = LocalSettings.settingsContainer.Values["HoppieLOGONCodeActual"] as string;
        }


        private async void SettingsVATISPathSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                settingsVATISPathTextBox.Text = folder.Name;
                localSettings.Values["vATISRequiredVersion"] = folder.Name.ToString();
            }
            else
            {
                settingsVATISPathTextBox.Text = "vATIS not found. Please select the installation directory from the right button.";
            }
        }

        private async void SettingsAFVPathSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };

            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                settingsAFVPathTextBox.Text = folder.Name;
                localSettings.Values["AFVRequiredVersion"] = folder.Name.ToString();
            }
            else
            {
                settingsAFVPathTextBox.Text = "Audio For VATSIM not found. Please select the installation directory from the right button.";
            }
        }

        // needs improvement
        private async void SettingsESPathSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, MainWindow._hWnd);

            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();

            if (folder == null)
            {
                settingsESPathTextBox.Text = "Please select a path";
            }
            else
            {
                if (folder.Path.Contains("Euroscope"))
                {
                    settingsESPathTextBox.Text = $"{folder?.Path + "\\EuroScope.exe"}";
                    settingsESPathTextBox.Foreground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    settingsESPathTextBox.Text = "Please select the correct path";
                }
            }

        }

        // Update colors to reflect that the path is not found.
        // When App wide settings or properties are implemented, fix this to check if PATH for the applications exists instead of string contains...
        private void UpdateColors()
        {
            if (settingsESPathTextBox.Text.Contains("not found"))
            {
                settingsESPathTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                settingsESPathTextBox.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (settingsAFVPathTextBox.Text.Contains("not found"))
            {
                settingsAFVPathTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                settingsAFVPathTextBox.Foreground = new SolidColorBrush(Colors.Green);
            }
            
            if (settingsVATISPathTextBox.Text.Contains("not found"))
            {
                settingsVATISPathTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                settingsVATISPathTextBox.Foreground = new SolidColorBrush(Colors.Green);
            }
        }


        /*
         *  Event Handling Section
         */
        // event to handle inputs and not allow any digits
        private void SettingsIDEditBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            var currentPosition = settingsVatsimCIDBox.SelectionStart - 1;
            var text = ((TextBox)sender).Text;

            // Using good old regex to filter out the text entered
            var regex = new Regex("^[0-9]*$");

            if (!regex.IsMatch(text))
            {
                // find the index of the char ([^0-9])
                var foundChar = Regex.Match(settingsVatsimCIDBox.Text, @"[^0-9]");
                if (foundChar.Success)
                {
                    settingsVatsimCIDBox.Text = settingsVatsimCIDBox.Text.Remove(foundChar.Index, 1);
                }

                settingsVatsimCIDBox.Select(currentPosition, 0);
            }
        }

        // save button click event handler
        private async void SettingsPageSaveButton_Click(object sender, RoutedEventArgs e)
        {

            settingsPageSaveButton.IsEnabled = false;

            LocalSettings.settingsContainer.Values["VATSIM_ID"] = settingsVatsimCIDBox.Text;
            LocalSettings.settingsContainer.Values["HoppieLOGONCode"] = settingsHoppieRichEditBox.Text;
            await Task.Delay(500);

            LocalSettings.settingsContainer.Values["EuroScopePath"] = settingsESPathTextBox.Text;
            LocalSettings.settingsContainer.Values["vATISPath"] = settingsVATISPathTextBox.Text;
            await Task.Delay(50);

            LocalSettings.settingsContainer.Values["AFVPath"] = settingsAFVPathTextBox.Text;

            await Task.Delay(50);
            settingsPageSaveButton.IsEnabled = true;
        }

        // Disable button for 3 seconds to avoid over pressing
        private async void GetHoppieLOGONCodeButton_Click(object sender, RoutedEventArgs e)
        {
            App.log.Info("getHoppieLogonCode button pressed");
            try
            {
                // You can update the UI because
                // the Click event will use the main thread.
                getHoppieLOGONCodeButton.IsEnabled = false;

                List<Task> tasks = new()
                {
                    // The main thread will be released here until
                    // LaunchUriAsync returns.
                    WindowsRuntimeSystemExtensions.AsTask(Launcher.LaunchUriAsync(new Uri("https://www.hoppie.nl/acars/system/register.html"))),
                    Task.Delay(3000)
                };
                await Task.WhenAll(tasks);
                // The main thread will be back here.
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error!",
                    Content = "Error when trying to open Hoppie ACARS system register page.",
                    CloseButtonText = "OK",
                };

                _ = await dialog.ShowAsync();

                App.log.Error("Error when trying to open Hoppie register page.", ex);
            }
            finally
            {
                // Enable the button even if an exception is thrown.
                getHoppieLOGONCodeButton.IsEnabled = true;
            }
        }

        // No need to allow paste as it could break input mask and formatting
        private void SettingsIDEditBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            ((TextBox)sender).Undo();
        }

        internal ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    }
}
