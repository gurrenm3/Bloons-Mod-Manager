using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.System;
using System.Diagnostics;
using System.IO;

namespace Bloons_Mod_Manager.Lib
{
    public class Launcher
    {
        private Platform _platform;
        private GameType _gameType;
        private GameInfo _gameInfo;

        public Launcher(GameType gameType, Platform platform)
        {
            _platform = platform;
            _gameType = gameType;
            _gameInfo = GameInfo.GetGame(gameType);
        }

        public void MoveUnusedMods()
        {
            foreach (var mod in SessionData.instance.AllMods)
            {
                if (!mod.IsActivated() && !mod.IsInUnloadFolder())
                {
                    mod.MoveToUnusedFolder();
                }
                else if (mod.IsActivated() && mod.IsInUnloadFolder())
                {
                    mod.MoveToModsFolder();
                }
            }
        }

        public void Launch()
        {
            if (_platform == Platform.PC)
            {
                if (SystemUtil.IsProgramRunning(_gameInfo.ProcName, out Process process))
                {
                    Logger.Log("The game is already open. Please close the game to continue", OutputType.ConsoleAndMsgBox);
                    return;
                }

                Process.Start("steam://rungameid/" + _gameInfo.SteamID);
            }
        }
    }
}
