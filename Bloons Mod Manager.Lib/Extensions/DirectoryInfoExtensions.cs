using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Bloons_Mod_Manager.Lib.Extensions
{
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Returns all Files in this directory that reference MelonLoader.dll or MelonLoader.ModHandler.dll
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static FileInfo[] GetAllMelonMods(this DirectoryInfo directoryInfo)
        {
            var files = directoryInfo.GetFiles();
            if (!files.Any())
                return null;

            return Array.FindAll(files, file => file.IsMelonMod());
        }
    }
}
