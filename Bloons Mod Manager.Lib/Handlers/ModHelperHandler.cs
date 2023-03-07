using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.Extensions;
using Bloons_Mod_Manager.Lib.Web;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Bloons_Mod_Manager.Lib.Handlers
{
    public class ModHelperHandler
    {
        const string githubReleaseURL = "https://api.github.com/repos/gurrenm3/BTD-Mod-Helper/releases";

        public GameType Game { get; set; }

        public ModHelperHandler(GameType game)
        {
            Game = game;
        }

        public async Task<bool> IsUpdateAsync()
        {
            string modHelperPath = GetModHelperPath(Game);
            Logger.Log(modHelperPath);
            if (!File.Exists(modHelperPath))
                return true;

            return false; // No longer checking versions. Mod Helper checks for updates on its own

            var updater = new UpdateChecker(githubReleaseURL);
            var releaseInfo = await updater.GetReleaseInfoAsync(githubReleaseURL);
            var latestRelease = releaseInfo[0];

            var versionNumber = FileVersionInfo.GetVersionInfo(modHelperPath).FileVersion;
            return updater.IsUpdate(versionNumber, latestRelease);
        }

        public static string GetModHelperPath(GameType game)
        {
            return game.GetGameInfo().ModsDir + "\\" + GetModHelperName(game);
        }

        public static string GetModHelperName(GameType game)
        {
            string name = null;
            switch (game)
            {
                case GameType.BloonsTD6:
                    name = "Btd6ModHelper.dll";
                    break;
                case GameType.BloonsAdventureTime:
                    name = "BloonsAT Mod Helper.dll";
                    break;
                default:
                    break;
            }
            return name;
        }
    }
}
