using Bloons_Mod_Manager.Lib.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bloons_Mod_Manager.Lib.Extensions
{
    public static class GameInfoExtensions
    {
        public static List<FileInfo> GetAllMelonMods(this GameInfo gameInfo)
        {
            if (!gameInfo.Game.IsMelonloaderGame())
                return null;

            List<FileInfo> melonMods = new List<FileInfo>();
            var mods = new DirectoryInfo(gameInfo.ModsDir)?.GetAllMelonMods();
            if (mods != null) melonMods.AddRange(mods);

            mods = new DirectoryInfo(gameInfo.UnusedModsDir)?.GetAllMelonMods();
            if (mods != null) melonMods.AddRange(mods);

            return melonMods;
        }
    }
}
