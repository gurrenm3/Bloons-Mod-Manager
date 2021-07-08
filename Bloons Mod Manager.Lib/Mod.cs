using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.Extensions;
using System;
using System.IO;

namespace Bloons_Mod_Manager.Lib
{
    public class Mod
    {
        public GameType Game { get; set; } = GameType.None;
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool IsOlderMelonMod { get; set; }
        private GameInfo _gameInfo;

        public Mod(GameType game, FileInfo fileInfo)
        {
            Game = game;
            FilePath = fileInfo.FullName;
            Name = fileInfo.Name;
            IsOlderMelonMod = fileInfo.IsOlderMelonMod();
            _gameInfo = GameInfo.GetGame(Game);
        }

        public Mod(GameType game, string filePath)
        {
            Game = game;
            FilePath = filePath;
            Name = new FileInfo(filePath).Name;
        }

        public bool IsInUnloadFolder()
        {
            return FilePath.Contains(Game.GetGameInfo().UnusedModsDir);
        }

        public bool IsActivated()
        {
            return SessionData.instance.ActivatedMods.Contains(this);
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
                Activate();
            else
                DeActivate();
        }

        private void Activate()
        {
            if (!IsActivated())
                SessionData.instance.ActivatedMods.Add(this);
        }

        private void DeActivate()
        {
            if (IsActivated())
                SessionData.instance.ActivatedMods.Remove(this);
        }

        public void MoveToUnusedFolder()
        {
            if (!File.Exists(FilePath))
            {
                Logger.Log("Warning. You tried moving a mod to the Unused folder, but the mod couldn't be found", OutputType.Console);
                return;
            }

            File.Move(FilePath, _gameInfo.UnusedModsDir + "\\" + Name);
            FilePath = _gameInfo.UnusedModsDir + "\\" + Name;
        }

        public void MoveToModsFolder()
        {
            if (!File.Exists(FilePath))
            {
                Logger.Log("Warning. You tried moving a mod to the Mods folder, but the mod couldn't be found", OutputType.Console);
                return;
            }

            File.Move(FilePath, _gameInfo.ModsDir + "\\" + Name);
            FilePath = _gameInfo.ModsDir + "\\" + Name;
        }
    }
}
