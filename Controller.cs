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
using System.Collections.Generic;
using Microsoft.UI.Xaml.Shapes;

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
         * 0 installed but ES needs to be updated to 3.2.3 (meaning installed version is lower)
         * 1 installed and correct version
         * 2 installed but ES needs to be restored to 3.2.3 (meaning installed version is higher)
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

        internal static int ControlIfSectorFilesInstalled()
        {
            string sectorFilesDefaultLocation = gitSectorFilesPath;

            if (Directory.Exists(sectorFilesDefaultLocation + "\\LTXX"))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        
        internal static void UpdateSectorFilesCMD()
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
            p.StandardInput.WriteLine("git pull");
        }

        internal static void InstallSectorFiles()
        {
            string path = System.IO.Path.Combine(gitSectorFilesPath.Remove(40), "sector-files");
            Directory.CreateDirectory(path);

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

        /*
         * This function reads and creates profıles for each sector
         * The name of the file can/will be changed, if it doesn't match it will fail
         */
        internal static void ReadProfiles()
        {
            string profileFileName = "Profiles.txt";

            // need more verification here since this file can be edited easily, reading wrong data will screw the parsing completely
            string profileFilePath = "not generated";
            try
            {
                profileFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"sector-files\LTXX\Settings\", profileFileName);
            }
            catch (Exception e)
            {
                App.log.Error("Path combination operation failed. Currently the profileFileName is " + profileFileName + " and the profileFilePath is " + profileFilePath + ".\n" + e.ToString());
                Process.GetCurrentProcess().Kill();
                throw;
            }

            // declare the lines array and initialize with an empty one
            string[] lines = Array.Empty<string>();

            // read all lines
            try
            {
                lines = File.ReadAllLines(profileFilePath);
            }
            catch (Exception e)
            {
                App.log.Error("Error during File.ReadAllLines() operation ont the profiles data. The file sent for parsing is " + profileFileName + " and generated path was " + profileFilePath + ".\nThe exception thrown is: " + e.ToString());
                Process.GetCurrentProcess().Kill();
            }

            // check if lines array is empty or has elements, if empty, break completely
            if (lines.Length == 0)
            {
                App.log.Error("could not generate the lines array, exiting");
                Process.GetCurrentProcess().Kill();
            }

            // starting index i from 1 since the first line is just PROFILE
            for (int i = 1; i < lines.Length; i += 4)
            {
                Profile temp = new Profile();
                if (lines[i] == "END")
                {
                    break;
                }
                if (!string.IsNullOrEmpty(lines[i]))
                {
                    temp.id = i;

                    if (lines[i].Contains("PROFILE:"))
                    {
                        string[] innerLines = lines[i].Split(new char[] { ':' });
                        temp.positionName = innerLines[1];
                        temp.range = UInt16.Parse(innerLines[2]); //parse and cast string to UINT16 (which is unsigned int16)
                        UInt16 facilityID = UInt16.Parse(innerLines[3]);
                        
                        /*
                         * FOR FUTURE VERSIONS
                         * If the facility ID is incorrect, that means the file is corrupted, this can force sector update
                         * for now, just log the error and kill
                         */
                        if (facilityID < 0 || facilityID > 6)
                        {
                            App.log.Error("Facility ID read is invalid. Please check " + profileFileName + " file since it is corrupted");
                            Process.GetCurrentProcess().Kill();
                        }

                        temp.facility = (Facility)facilityID; // first parse the string to an unsigned 16 bit integer then cast it to facility ENUM (facility can only have 
                    }
                    
                    if (lines[i + 1].Contains("ATIS2")) 
                    {
                        string[] innerLines = lines[i + 1].Split(new char[] { ':' });
                        temp.ATIS2 = innerLines[1];
                    }
                    
                    if (lines[i + 2].Contains("ATIS3"))
                    {
                        string[] innerLines = lines[i + 2].Split(new char[] { ':' });
                        temp.ATIS3 = innerLines[1];
                    }
                    
                    if (lines[i + 3].Contains("ATIS4"))
                    {
                        string[] innerLines = lines[i + 3].Split(new char[] { ':' });
                        temp.ATIS4 = innerLines[1];
                    }
                    LocalSettings.profileList.Add(temp);
                }
                else
                {
                    App.log.Error("The profile parsing has failed at the block starting with " + i + ". Here is the dumped file: \n" + String.Join('\n', lines));
                    Process.GetCurrentProcess().Kill();
                }
            }
        }


        /*
         *          - MEMBER VARIABLES -
         * Please put the member variables to under this section here
         */
        internal static string gitSectorFilesPath = "C:\\Users\\" + Environment.UserName.ToString() + "\\AppData\\Roaming\\sector-files";

    }
}
