using Bloons_Mod_Manager.Lib.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bloons_Mod_Manager.Lib
{
    /// <summary>
    /// Manages persistence data related to user, such as game locations, if they are a new user, etc.
    /// This data is not intended to be configurable, therefore this is not the same as config/settings
    /// </summary>
    public class UserData
    {
        public static Action OnUserDataLoaded;
        public static string MainProgramName;
        public static string MainProgramExePath;
        public static string MainSettingsDir;
        public static string UserDataFilePath;

        private static UserData _instance;

        public static UserData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadUserData();
                    OnUserDataLoaded?.Invoke();
                }

                return _instance;
            }
            set { _instance = value; }
        }


        /// <summary>
        /// Manages the last known version of the executing program
        /// </summary>
        public string MainProgramVersion { get; set; } = "";

        /// <summary>
        /// Has the user just updated the executing program?
        /// </summary>
        public bool RecentUpdate { get; set; } = true;

        /// <summary>
        /// Is this this a new user?
        /// </summary>
        public bool NewUser { get; set; } = true;


        /// <summary>
        /// BTD5 Data
        /// </summary>
        #region BTD5
        /*private static GameInfo btd5 = GameInfo.GetGame(GameType.BloonsTD5);
        public string BTD5Dir { get; set; } = btd5.GameDir;
        public string BTD5Version { get; set; } = FileVersionInfo.GetVersionInfo(btd5.GameDir + "\\" + btd5.EXEName)?.FileVersion;
        public string BTD5BackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + btd5.Game.ToString();*/
        #endregion

        /// <summary>
        /// BTDB Data
        /// </summary>
        #region BTDB
        /*private static GameInfo btdb = GameInfo.GetGame(GameType.BloonsTDBattles);
        public string BTDBDir { get; set; } = btdb.GameDir;
        public string BTDBVersion { get; set; } = FileVersionInfo.GetVersionInfo(btdb.GameDir + "\\" + btdb.EXEName)?.FileVersion;
        public string BTDBBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + btdb.Game.ToString();*/
        #endregion

        /// <summary>
        /// Bloons Monkey City Data
        /// </summary>
        #region Monkey City
        /*private static GameInfo bmc = GameInfo.GetGame(GameType.BloonsMonkeyCity);
        public string BMCDir { get; set; } = bmc.GameDir;
        public string BMCVersion { get; set; } = FileVersionInfo.GetVersionInfo(bmc.GameDir + "\\" + bmc.EXEName)?.FileVersion;
        public string BMCBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + bmc.Game.ToString();*/
        #endregion

        /// <summary>
        /// BTD6 Data
        /// </summary>
        #region BTD6
        private static GameInfo btd6 = GameInfo.GetGame(GameType.BloonsTD6);
        public string BTD6Dir { get; set; } = btd6.GameDir;
        public string BTD6Version { get; set; } //= FileVersionInfo.GetVersionInfo(btd6.GameDir + "\\" + btd6.EXEName)?.FileVersion;
        #endregion

        /// <summary>
        /// Bloons Adventure Time Data
        /// </summary>
        #region BTDAT
        private static GameInfo btdat = GameInfo.GetGame(GameType.BloonsAdventureTime);
        public string BTDATDir { get; set; } = btdat.GameDir;
        public string BTDATVersion { get; set; }// = FileVersionInfo.GetVersionInfo(btdat.GameDir + "\\" + btdat.EXEName)?.FileVersion;
        #endregion

        /// <summary>
        /// NKArchive Data
        /// </summary>
        #region NK Archive
        /*private static GameInfo nkArchive = GameInfo.GetGame(GameType.NKArchive);
        public string NKArchiveDir { get; set; } = nkArchive.GameDir;
        public string NKArchiveVersion { get; set; } = FileVersionInfo.GetVersionInfo(nkArchive.GameDir + "\\" + nkArchive.EXEName)?.FileVersion;
        public string NKArchiveBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + nkArchive.Game.ToString();*/


        public List<string> PreviousProjects { get; set; }
        #endregion


        #region Constructors
        public UserData()
        {
            if (String.IsNullOrEmpty(MainSettingsDir))
                MainSettingsDir = Environment.CurrentDirectory;

            if (String.IsNullOrEmpty(MainProgramVersion))
                MainProgramVersion = FileVersionInfo.GetVersionInfo(MainProgramExePath).FileVersion;

            if (!Directory.Exists(MainSettingsDir))
                Directory.CreateDirectory(MainSettingsDir);

            if (String.IsNullOrEmpty(UserDataFilePath))
                UserDataFilePath = MainSettingsDir + "\\userdata.json";

            if (PreviousProjects == null)
                PreviousProjects = new List<string>();

            OnUserDataLoaded += () =>
            {
                // BTD6 stuff
                if (string.IsNullOrEmpty(btd6.GameDir) && Directory.Exists(BTD6Dir))
                    btd6.GameDir = BTD6Dir;
                else if (string.IsNullOrEmpty(BTD6Dir) && Directory.Exists(btd6.GameDir))
                    BTD6Dir = btd6.GameDir;

                string btd6Exe = $"{btd6.GameDir}\\{btd6.EXEName}";
                if (File.Exists(btd6Exe))
                {
                    BTD6Version = FileVersionInfo.GetVersionInfo(btd6Exe)?.FileVersion;
                    btd6.ModsDir = $"{btd6.GameDir}\\Mods";
                    btd6.UnusedModsDir = $"{btd6.GameDir}\\Mods\\Unused Mods";

                    Directory.CreateDirectory(btd6.ModsDir);
                    Directory.CreateDirectory(btd6.UnusedModsDir);
                }


                // BTDAT stuff
                if (string.IsNullOrEmpty(btdat.GameDir) && Directory.Exists(BTDATDir))
                    btdat.GameDir = BTDATDir;
                else if (string.IsNullOrEmpty(BTDATDir) && Directory.Exists(btdat.GameDir))
                    BTDATDir = btdat.GameDir;

                string btdATExe = $"{btdat.GameDir}\\{btdat.EXEName}";
                if (File.Exists(btdATExe))
                {
                    BTDATVersion = FileVersionInfo.GetVersionInfo(btdATExe)?.FileVersion;
                    btdat.ModsDir = $"{btdat.GameDir}\\Mods";
                    btdat.UnusedModsDir = $"{btdat.GameDir}\\Mods\\Unused Mods";

                    Directory.CreateDirectory(btdat.ModsDir);
                    Directory.CreateDirectory(btdat.UnusedModsDir);
                }
            };
        }

        #endregion

        /// <summary>
        /// Open the main settings directory
        /// </summary>
        public static void OpenSettingsDir()
        {
            /*if (Instance == null)
                Instance = new UserData();*/
            
            Directory.CreateDirectory(MainSettingsDir);

            Process.Start(MainSettingsDir);
        }

        /// <summary>
        /// Load userdata from file
        /// </summary>
        /// <returns>The loaded userdata</returns>
        private static UserData LoadUserData()
        {
            /*if (Instance == null)
                Instance = new UserData();*/

            UserData user;

            if (!File.Exists(UserDataFilePath))
            {
                user = new UserData();
                SaveUserData(user);
                return user;
            }

            string json = File.ReadAllText(UserDataFilePath);
            if (string.IsNullOrEmpty(json))
            {
                Logger.Log("Userdata has invalid json, generating a new one.");
                user = new UserData();
                SaveUserData(user);
                return user;
            }

            user = JsonConvert.DeserializeObject<UserData>(json);
            return user;
        }

        /// <summary>
        /// Save userdata to file
        /// </summary>
        public static void SaveUserData(UserData instance = null)
        {
            /*if (Instance == null)
                Instance = LoadUserData();*/

            if (instance == null)
                instance = Instance;

            string output = JsonConvert.SerializeObject(instance, Formatting.Indented);

            StreamWriter serialize = new StreamWriter(UserDataFilePath, false);
            serialize.Write(output);
            serialize.Close();
        }
    }
}