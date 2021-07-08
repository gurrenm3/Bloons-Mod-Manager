using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.Web;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bloons_Mod_Manager.Lib.Handlers
{
    public class MelonLoaderHandler
    {
        public const string githubAPIReleaseURL = "https://api.github.com/repos/HerpDerpinstine/MelonLoader/releases";
        private static readonly GameType[] MelonLoaderGames = new GameType[3] { GameType.BloonsTD6, GameType.BloonsTDBattles2, GameType.BloonsAdventureTime };

        public GameType Game { get; set; }
        public GameInfo GameData { get; set; }

        private string _melonHandlerPath;
        private string _melonLoaderDirectory;

        public MelonLoaderHandler(GameType game)
        {
            Game = game;
            GameData = GameInfo.GetGame(game);
            _melonLoaderDirectory = GameData.GameDir + "\\MelonLoader";
            _melonHandlerPath = _melonLoaderDirectory + "\\MelonLoader.dll";
        }

        public async Task<bool> IsUpdateAvailableAsync()
        {
            if (!DoesMelonLoaderExist())
                return true;

            var updater = new UpdateChecker(githubAPIReleaseURL);
            var releaseInfo = await updater.GetReleaseInfoAsync(githubAPIReleaseURL);
            var latestRelease = releaseInfo[0];

            var versionNumber = FileVersionInfo.GetVersionInfo(_melonHandlerPath).ProductVersion;
            return updater.IsUpdate(versionNumber, latestRelease);
        }

        public bool DoesMelonLoaderExist()
        {
            string melonloaderVersionDLL = $"{GameData.GameDir}\\version.dll";
            return File.Exists(melonloaderVersionDLL) && File.Exists(_melonHandlerPath);
        }

        public static bool IsMelonloaderGame(GameType game)
        {
            return MelonLoaderGames.Contains(game);
        }
    }
}
