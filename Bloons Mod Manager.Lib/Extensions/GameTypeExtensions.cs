using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.Handlers;

namespace Bloons_Mod_Manager.Lib.Extensions
{
    public static class GameTypeExtensions
    {
        public static bool IsMelonloaderGame(this GameType game)
        {
            return MelonLoaderHandler.IsMelonloaderGame(game);
        }

        public static bool IsInstalled(this GameType game)
        {
            return SteamUtils.IsGameInstalled(game);
        }

        public static GameInfo GetGameInfo(this GameType game)
        {
            return GameInfo.GetGame(game);
        }
    }
}
