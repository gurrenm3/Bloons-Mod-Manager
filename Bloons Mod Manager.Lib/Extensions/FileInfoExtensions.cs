using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bloons_Mod_Manager.Lib.Extensions
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Get all Assembly References from this FileInfo. Returns null if there are none
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static AssemblyName[] GetAllReferences(this FileInfo fileInfo)
        {
            try { return Assembly.LoadFrom(fileInfo.FullName)?.GetReferencedAssemblies(); }
            catch (Exception ex) 
            {
                if (ex.Message.Contains("HRESULT: 0x80131515"))
                {
                    Logger.Log($"The file you tried accessing is blocked. Please Unblock \"{fileInfo.FullName}\" to continue." +
                        $"\n\nYou can do this by Right-Clicking on the file, clicking Properties, and then making sure that " +
                        $"\"Unblock\" is checked.\n\nA picture will be opened to show you how to do it.", OutputType.MsgBox);

                    if (!SessionData.instance.UnblockFilePictureShown)
                    {
                        Process.Start("https://github.com/gurrenm3/Bloons-Mod-Manager/blob/master/how%20to%20unblock%20a%20file.png");
                        SessionData.instance.UnblockFilePictureShown = true;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns whether or not this File has a reference to the newer MelonLoader.dll or the older MelonLoader.ModHandler.dll
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsMelonMod(this FileInfo fileInfo)
        {
            return fileInfo.IsNewerMelonMod() || fileInfo.IsOlderMelonMod();
        }

        /// <summary>
        /// Returns whether or not this File has a reference to the newer MelonLoader.dll (For MelonLoader 3.0 and up)
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsNewerMelonMod(this FileInfo fileInfo)
        {
            var references = fileInfo.GetAllReferences();
            if (references is null) return false;

            return references.Any(reference => reference.Name == "MelonLoader" || reference.Name == "MelonLoader.dll");
        }

        /// <summary>
        /// Returns whether or not this File has a reference to the older MelonLoader.ModHandler.dll (For MelonLoader 2.7.4 and below)
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsOlderMelonMod(this FileInfo fileInfo)
        {
            var references = fileInfo.GetAllReferences();
            if (references is null) return false;

            return references.Any(reference => reference.Name == "MelonLoader.ModHandler");
        }
    }
}