using Bloons_Mod_Manager.Lib.Enums;
using System.Collections.Generic;

namespace Bloons_Mod_Manager.Lib
{
    /// <summary>
    /// Contains game specific info for each game. Includes exe and process names, directories, save files, passwords, etc.
    /// </summary>
    public class GameInfo
    {
        #region Properties

        private static List<GameInfo> games;
        /// <summary>
        /// A singleton list containing all of the games, each with their relevant info
        /// </summary>
        public static List<GameInfo> Games
        {
            get
            {
                if (games == null)
                    games = new GameInfo().InitializeGameInfo();

                return games;
            }
        }

        /// <summary>
        /// The GameType for this specific game. Example: GameType.BTD5
        /// </summary>
        public GameType Game { get; set; }

        /// <summary>
        /// The name of the game's exe. Example: "BTD5-Win.exe"
        /// </summary>
        public string EXEName { get; set; }

        /// <summary>
        /// The main directory for the game, same as the one that contains the exe. 
        /// Example: "C:\Program Files (x86)\Steam\steamapps\common\BloonsTD5"
        /// </summary>
        public string GameDir { get; set; }

        /// <summary>
        /// The location where mods will be stored
        /// </summary>
        public string ModsDir { get; set; }

        /// <summary>
        /// The location unused mods will be stored
        /// </summary>
        public string UnusedModsDir { get; set; }

        /// <summary>
        /// The directory that contains the save files. Example: "C:\Program Files (x86)\Steam\userdata\50223118\306020"
        /// </summary>
        public string SaveDir { get; set; }

        /// <summary>
        /// The name of the game's process, as seen in task manager. 
        /// Example: Bloons TD5 Game
        /// </summary>
        public string ProcName { get; set; }

        /// <summary>
        /// The name of the game's jet file, as seen in it's Assets folder. Example: "BTD5.jet"
        /// </summary>
        public string JetName { get; set; }

        /// <summary>
        /// The password for the game's jet file. Example: "Q%_{6#Px]]"
        /// </summary>
        public string JetPassword { get; set; }

        /// <summary>
        /// The exact path to the jet file
        /// </summary>
        public string JetPath { get; set; }

        /// <summary>
        /// The SteamApp ID for this game
        /// </summary>
        public ulong SteamID { get; set; }

        #endregion


        /// <summary>
        /// Get a specific game from the Games list
        /// </summary>
        /// <param name="type">The game you want to get</param>
        /// <returns></returns>
        public static GameInfo GetGame(GameType type)
        {
            return Games.Find(g => g.Game == type);
        }

        /// <summary>
        /// Initialize game info for each game in GameType.
        /// </summary>
        private List<GameInfo> InitializeGameInfo()
        {
            var btd5 = new GameInfo
            {
                Game = GameType.BloonsTD5,
                EXEName = "BTD5-Win.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BloonsTD5),
                SaveDir = "",
                ProcName = "BTD5-Win",
                JetName = "BTD5.jet",
                JetPassword = "Q%_{6#Px]]",
                JetPath = GameDir + "\\Assets\\" + JetName,
                SteamID = SteamUtils.GetGameID(GameType.BloonsTD5)
            };

            var btdb = new GameInfo
            {
                Game = GameType.BloonsTDBattles,
                EXEName = "Battles-Win.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BloonsTDBattles),
                SaveDir = "",
                ProcName = "Battles-Win",
                JetName = "data.jet",
                JetPassword = "",
                JetPath = GameDir + "\\Assets\\" + JetName,
                SteamID = SteamUtils.GetGameID(GameType.BloonsTDBattles)
            };

            var bmc = new GameInfo
            {
                Game = GameType.BloonsMonkeyCity,
                EXEName = "MonkeyCity-Win.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BloonsMonkeyCity),
                SaveDir = "",
                ProcName = "MonkeyCity-Win",
                JetName = "data.jet",
                JetPassword = "Q%_{6#Px]]",
                JetPath = GameDir + "\\Assets\\" + JetName,
                SteamID = SteamUtils.GetGameID(GameType.BloonsMonkeyCity)
            };

            var btd6 = new GameInfo
            {
                Game = GameType.BloonsTD6,
                EXEName = "BloonsTD6.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BloonsTD6),
                SaveDir = "",
                ProcName = "BloonsTD6",
                JetName = "BTD6.jet",
                JetPassword = "",
                SteamID = SteamUtils.GetGameID(GameType.BloonsTD6)
            };

            var btdat = new GameInfo
            {
                Game = GameType.BloonsAdventureTime,
                EXEName = "btdadventuretime.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BloonsAdventureTime),
                SaveDir = "",
                ProcName = "btdadventuretime",
                JetName = "",
                JetPassword = "",
                SteamID = SteamUtils.GetGameID(GameType.BloonsAdventureTime)
            };

            var nkArchive = new GameInfo
            {
                Game = GameType.NKArchive,
                EXEName = "Ninja Kiwi Archive.exe",
                GameDir = SteamUtils.GetGameDir(GameType.NKArchive),
                SaveDir = "",
                ProcName = "Ninja Kiwi Archive",
                JetName = "",
                JetPassword = "",
                SteamID = SteamUtils.GetGameID(GameType.NKArchive)
            };

            var games = new List<GameInfo>() { btd5, btdb, bmc, btd6, btdat, nkArchive };

            foreach (var item in games)
            {
                item.ModsDir = $"{item.GameDir}\\Mods";
                item.JetPath = item.GameDir;

                if (item.Game == GameType.BloonsTD6 || item.Game == GameType.BloonsAdventureTime || item.Game == GameType.BloonsTDBattles2)
                {
                    item.JetPath += "\\" + item.JetName;
                    item.UnusedModsDir = item.ModsDir + "\\Unused Mods";
                }
                else if (item.Game == GameType.BloonsTD5 || item.Game == GameType.BloonsTDBattles || item.Game == GameType.BloonsMonkeyCity)
                {
                    item.JetPath += "\\Assets\\" + item.JetName;
                    item.UnusedModsDir = item.ModsDir;
                }
                else
                    continue;

            }

            return games;
        }
    }
}
