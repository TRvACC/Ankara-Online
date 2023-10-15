using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Windows.Storage;
using Windows.System.Profile;

namespace Ankara_Online
{
    internal static class LocalSettings
    {
        static LocalSettings()
        {
            settingsContainer = ApplicationData.Current.LocalSettings;
            uiElementsDictionary = new Dictionary<string, string>();
            LTFM_METAR_PARSE_ERROR = true;
            LTFM_PRS_PARSE_ERROR = true;
            LTFJ_METAR_PARSE_ERROR = true;
            LTFJ_PRS_PARSE_ERROR = true;
            LTAI_METAR_PARSE_ERROR = true;
            LTAI_PRS_PARSE_ERROR = true;
            correctEuroScopeVersion = false;
            correctAFVVersion = false;
            correctVATISVersion = false;
            correctSectorFilesVersion = false;
            profileList = new List<Profile>();
        }

        internal static async void CheckIfSettingsExists()
        {
            if (!settingsContainer.Containers.ContainsKey("VATSIM_ID"))
            {

                App.log.Info("Local Settings does not exists. Creating settings for VATSIM ID, Hoppie LOGON Code, App Version, Required and Installed paths.");
                settingsContainer.Values["VATSIM_ID"] = null;
                settingsContainer.Values["VATSIM_IDactual"] = null;
                settingsContainer.Values["UserRealName"] = null;
                settingsContainer.Values["VATSIM_Password"] = null;
                settingsContainer.Values["HoppieLOGONCode"] = null;
                settingsContainer.Values["HoppieLOGONCodeActual"] = null;
                settingsContainer.Values["AppVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                settingsContainer.Values["EuroScopePath"] = Controller.GetEuroScopePath();
                settingsContainer.Values["SectorFilesPath"] = null;
                settingsContainer.Values["vATISPath"] = Controller.GetVATISPath();
                settingsContainer.Values["AFVPath"] = Controller.GetAFVPath();
                
                settingsContainer.Values["EuroScopeRequiredVersion"] = Controller.GetEuroScopeRequiredVersion();
                settingsContainer.Values["SectorFilesRequiredVersion"] = null;
                settingsContainer.Values["AFVRequiredVersion"] = Controller.GetAFVRequiredVersion();
                settingsContainer.Values["vATISRequiredVersion"] = await Controller.GetVATISRequiredVersionAsync();

                settingsContainer.Values["EuroScopeInstalledVersion"] = Controller.GetEuroScopeInstalledVersion();
                settingsContainer.Values["SectorFilesInstalledVersion"] = null;
                settingsContainer.Values["AFVInstalledVersion"] = Controller.GetAFVInstalledVersion();
                settingsContainer.Values["vATISInstalledVersion"] = Controller.GetVATISInstalledVersion();

                settingsContainer.Values["AFV_VERSION_CHECK_URL"] = "https://github.com/vatsimnetwork/afv-clients/blob/main/clientversion.xml";
                settingsContainer.Values["vATIS_VERSION_CHECK_JSON"] = "https://vatis.clowd.io/api/v4/VersionCheck";
                // settingsContainer.Values["TRvACC_SMART_API"] = "https://rasat.trvacc.net/metar/";
            }
        }

        internal static IDictionary<string, string> uiElementsDictionary;
        internal static bool LTFM_METAR_PARSE_ERROR;
        internal static bool LTFM_PRS_PARSE_ERROR;
        internal static bool LTFJ_METAR_PARSE_ERROR;
        internal static bool LTFJ_PRS_PARSE_ERROR;
        internal static bool LTAI_METAR_PARSE_ERROR;
        internal static bool LTAI_PRS_PARSE_ERROR;

        internal static bool correctEuroScopeVersion;
        internal static bool correctAFVVersion;
        internal static bool correctVATISVersion;
        internal static bool correctSectorFilesVersion;

        /*
         * profileList is a List which contains Profile struct typed elements for each position
         */
        internal static List<Profile> profileList;

        internal static ApplicationDataContainer settingsContainer;
        internal static readonly string DEFAULT_ES_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\EuroScope";
        internal static readonly string DEFAULT_VATIS_PATH = Environment.GetEnvironmentVariable("LocalAppData") + @"\vATIS-4.0";

    }

    public struct Profile
    {
        internal int id;
        internal string positionName;
        internal UInt16 range;
        internal Facility facility;
        internal string ATIS2;
        internal string ATIS3;
        internal string ATIS4;
    }

    // an enum to designate type of facility of the sector
    enum Facility
    {
        Observer, // 0 = Observer
        Flight_Service_Station, // 1 = Flight Service Station
        Clearance_Delivery, // 2 = Clearance/Delivery
        Ground, // 3 = Ground
        Tower, // 4 = Tower
        Approach_Departure, // 5 = Approach/Departure
        Center // 6 = Center
    }
}
