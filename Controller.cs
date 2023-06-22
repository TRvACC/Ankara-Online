using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.ComponentModel.Design;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Ankara_Online
{
    /*
     * This class contains all the controller methods
     * as controller might be called from multiple places, having a single solution saves time, space and is easier for project management
     */
    internal class Controller
    {
        internal static bool EuroScopePathChecker()
        {
            try
            {
                return Directory.Exists(LocalSettings.settingsContainer.Values["EuroScopePath"] as string) && File.Exists((LocalSettings.settingsContainer.Values["EuroScopePath"] as string) + @"\EuroScope.exe");
            }
            catch (Exception e)
            {
                App.log.Error("Exception thrown at EuroScopePathChecker(). Exception thrown: " + e.ToString());
                return false;
            }
        }

        internal static bool vATISPathChecker()
        {
            try
            {
                // %localappdata%\vATIS-4.0
                return Directory.Exists(LocalSettings.settingsContainer.Values["vATISPath"] as string) && File.Exists((LocalSettings.settingsContainer.Values["vATISPath"] as string) + @"\\vATIS.exe");
            }
            catch (Exception e)
            {
                App.log.Error("Exception thrown at vATISPathChecker(). Exception thrown: " + e.ToString());
                return false;
            }
        }

        internal static bool AFVPathChecker()
        {
            try
            {
                return Directory.Exists(LocalSettings.settingsContainer.Values["AFVPath"] as string) && File.Exists((LocalSettings.settingsContainer.Values["AFVPath"] as string) + @"\AudioForVATSIM.exe");

            }
            catch (Exception e)
            {
                App.log.Error("Exception thrown at AFVPathChecker(). Exception thrown: " + e.ToString());
                return false;
            }
        }

        internal static bool SectorFilesPathChecker()
        {
            // until method is implemented
            return false;
        }
        
        internal static string GetApplicationPath(string applicationName,string containerKey)
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components"))
            {
                foreach (string keyname in key.GetSubKeyNames())
                {
                    RegistryKey tempKey = key.OpenSubKey(keyname);
                    var value = tempKey.GetValue(tempKey.GetValueNames()[0]).ToString();
                    var program = value.Substring(value.LastIndexOf('\\') + 1);
                    if (program == applicationName)
                    {
                        value = value.Remove(value.LastIndexOf("\\"));
                        LocalSettings.settingsContainer.Values[containerKey] = value;
                        return value;
                    }
                }
            }
            return string.Empty;
        }

        internal static string GetEuroScopePath()
        {
            try
            {
                if (Directory.Exists(LocalSettings.DEFAULT_ES_PATH) && File.Exists(LocalSettings.DEFAULT_ES_PATH + @"\EuroScope.exe"))
                {
                    return LocalSettings.DEFAULT_ES_PATH;
                }
                else
                {
                    return GetApplicationPath(@"EuroScope.exe", "EuroScopePath");
                }
            }
            catch (FileNotFoundException e)
            {
                App.log.Error("Exception thrown at GetEuroScopePath(). Could not detect Euroscope at this location. Exception thrown: " + e.ToString());
                return null;
            }
            catch (Exception e)
            {
                App.log.Error("Exception thrown at GetEuroScopePath(). Exception different than FileNotFoundException. Exception thrown: " + e.ToString());
                return null;
            }

            return null;
        }

        /*
        // for default installation
        internal static void SetEuroscopeInstalledVersion()
        {
            if (EuroScopePathChecker())
            {
                LocalSettings.settingsContainer.Values["EuroScopePath"] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Euroscope";
                LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] = FileVersionInfo.GetVersionInfo((LocalSettings.settingsContainer.Values["EuroScopePath"] as string) + @"\EuroScope.exe").FileVersion;
            }
        }

        /* 
         * use this if ES is installed somewhere else however this should not even be allowed
         * this function will probably be deleted in the future
         *
        internal static void SetEuroScopeInstalledVersion(string path)
        {
            LocalSettings.settingsContainer.Values["EuroScopePath"] = path;
            LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] = FileVersionInfo.GetVersionInfo(path + @"\EuroScope.exe").FileVersion;
        }
        */

        internal static string GetEuroScopeInstalledVersion()
        {
            return FileVersionInfo.GetVersionInfo((LocalSettings.settingsContainer.Values["EuroScopePath"] as string) + @"\EuroScope.exe").FileVersion;
        }

        internal static string GetEuroScopeRequiredVersion()
        {
            return "3.2.3.0";
        }

        internal static string GetVATISPath()
        {
            string path = null;
            try
            {
                if (Directory.Exists(LocalSettings.DEFAULT_VATIS_PATH) && File.Exists(LocalSettings.DEFAULT_VATIS_PATH + @"\Application\vATIS.exe"))
                {
                    path = LocalSettings.DEFAULT_VATIS_PATH;
                }
                else
                {
                    return GetApplicationPath(@"vATIS.exe", "vATISPath");
                }
            }
            catch (FileNotFoundException e)
            {
                App.log.Error("Exception thrown at GetVATISPath(). Could not detect vATIS at this location. Exception thrown: " + e.ToString());
                path = null;
            }
            catch (Exception e)
            {
                App.log.Error("Exception thrown at GetVATISPath(). Exception different than FileNotFoundException. Exception thrown: " + e.ToString());
                path = null;
            }

            return path;
        }

        internal static string GetVATISInstalledVersion()
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(LocalSettings.settingsContainer.Values["vATISPath"] + @"\Application\vATIS.exe").FileVersion;
            }
            catch (Exception e)
            {
                App.log.Error("Error when getting the file version of vATIS. Exception thrown at GetVATISInstalledVersion(), raised: " + e.ToString());
                return null;
            }
        }

        internal static async Task<string> GetVATISRequiredVersionAsync()
        {
            string vatisJSON = null;

            using HttpClient client = new HttpClient();

            try
            {
                App.log.Info("GET vATIS v4 API VersionCheck");
                vatisJSON = await client.GetStringAsync("https://vatis.clowd.io/api/v4/VersionCheck");
            }
            catch (Exception e)
            {
                App.log.Error("Error fetching vatis version check json from API. URL was https://vatis.clowd.io/api/v4/VersionCheck. Exception raised was: " + e.ToString());
                return vatisJSON;
            }
            App.log.Info("Successfully got return for VersionCheck");

            if (vatisJSON == null)
            {
                App.log.Error("vatisJsonObj is empty and Controller.VATISVersionCheckerAsync() was not able to do any version comparison");
                return vatisJSON;
            }

            dynamic vatisJsonObj = JsonConvert.DeserializeObject<dynamic>(vatisJSON);

            return vatisJsonObj["LatestVersion"].ToString();
        }

        internal static string GetAFVPath()
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            using (var key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components"))
            {
                foreach (string keyname in key.GetSubKeyNames())
                {
                    RegistryKey tempKey = key.OpenSubKey(keyname);
                    var value = tempKey.GetValue(tempKey.GetValueNames()[0]).ToString();
                    var program = value.Substring(value.LastIndexOf('\\') + 1);
                    if (program == "AudioForVATSIM.exe")
                    {
                        LocalSettings.settingsContainer.Values["AFVPath"] = value;
                        return value;
                    }
                }
            }

            return null;
        }

        internal static string GetAFVInstalledVersion()
        {
            return FileVersionInfo.GetVersionInfo(GetAFVPath()).FileVersion;
        }

        internal static string GetAFVRequiredVersion()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("https://raw.githubusercontent.com/vatsimnetwork/afv-clients/main/clientversion.xml");
            XmlNode version = doc.DocumentElement.SelectSingleNode("/item/version");
            XmlNode url = doc.DocumentElement.SelectSingleNode("/item/url");

            return version.InnerText;
        }

        /*
         * Check EuroScope version
         * Return values are: 
         * -2 for error
         * -1 not installed
         * 0 installed but ES needs to be updated to 3.2.2 (meaning installed version is lower)
         * 1 installed and correct version
         * 2 installed but ES needs to be restored to 3.2.2 (meaning installed version is higher)
         */
        internal static int EuroScopeVersionChecker()
        {
            if (LocalSettings.settingsContainer.Values["EuroScopeInstalledVersion"] as string != null)
            {
                var ESInstalledVersion = new Version(GetEuroScopeInstalledVersion());
                var ESRequiredVersion = new Version(GetEuroScopeRequiredVersion());
                if (ESInstalledVersion < ESRequiredVersion)
                {
                    LocalSettings.correctEuroScopeVersion = false;
                    return 0;
                }
                else if (ESInstalledVersion == ESRequiredVersion)
                {
                    LocalSettings.correctEuroScopeVersion = true;
                    return 1;
                }

                else if (ESInstalledVersion > ESRequiredVersion)
                {
                    LocalSettings.correctEuroScopeVersion = false;
                    return 2;
                }
            }
            else
            {
                LocalSettings.correctEuroScopeVersion = false;
                return -1;
            }
            LocalSettings.correctEuroScopeVersion = false;
            return -2;
        }

        /*
         * Check vATIS version
         * Return values are: 
         * -1 not installed
         * 0 installed but not correct version, needs to be updated
         * 1 installed and correct version
         */
        internal static async Task<int> VATISVersionCheckerAsync()
        {
            if (LocalSettings.settingsContainer.Values["vATISInstalledVersion"] as string == null)
            {
                LocalSettings.correctVATISVersion = false;
                return -1;
            }

            var VATISInstalledVersion = new Version(LocalSettings.settingsContainer.Values["vATISInstalledVersion"] as string);
            var VATISRequiredVersion = new Version(await GetVATISRequiredVersionAsync());

            if (VATISInstalledVersion == VATISRequiredVersion)
            {
                LocalSettings.correctVATISVersion = true;
                return 1;
            }
            else
            {
                LocalSettings.correctVATISVersion = false;
                return 0;
            }
        }

        /*
         * Check AFV version
         * Return values are: 
         * -1 not installed
         * 0 installed but not correct version, needs to be updated
         * 1 installed and correct version
         */
        internal static int AFVVersionChecker()
        {
            if (LocalSettings.settingsContainer.Values["AFVInstalledVersion"] as string == null)
            {
                LocalSettings.correctAFVVersion = false;
                return -1;
            }

            var requiredVersion = new Version(GetAFVRequiredVersion());
            var installedVersion = new Version(LocalSettings.settingsContainer.Values["AFVInstalledVersion"] as string);

            if (installedVersion == requiredVersion)
            {
                LocalSettings.correctAFVVersion = true;
                return 1;
            }
            else
            {
                LocalSettings.correctAFVVersion = false;
                return 0;
            }
        }

        // simple version checking from version file
        internal static void SectorFilesVersionCheckerSimple()
        {

        }

        // implement WebDAV here
        internal static void SectorFilesVersionCheckerComplete()
        {

        }

        internal static string userName = Environment.UserName;
        internal static string gitSfPath = "C:\\Users\\" + userName + "\\AppData\\Roaming\\sector-files";
        internal static int ControlIfSFInstalled()
        {
            string sfDefaultLocation = gitSfPath;

            if (Directory.Exists(sfDefaultLocation))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        internal static void UpdateSF()
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            p.StartInfo = info;
            p.Start();
            p.StandardInput.WriteLine("cd %APPDATA%\\sector-files");
            p.StandardInput.WriteLine("git checkout .");
        }

        internal static void InstallSF()
        {
            string path = System.IO.Path.Combine(Controller.gitSfPath.Remove(40), "sector-files");
            System.IO.Directory.CreateDirectory(path);

            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            p.StartInfo = info;
            p.Start();
            p.StandardInput.WriteLine("git clone https://github.com/TRvACC/sector-files.git %APPDATA%/sector-files 2>NUL");

        }
    }
}
