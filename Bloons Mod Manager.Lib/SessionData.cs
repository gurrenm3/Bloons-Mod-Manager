using Bloons_Mod_Manager.Lib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloons_Mod_Manager.Lib
{
    public class SessionData
    {
        public static SessionData instance = new SessionData();

        public bool UnblockFilePictureShown = false;
        public Platform CurrentPlatform { get; set; } = Platform.PC;
        public GameType CurrentGame { get; set; } = GameType.None;
        public List<GameType> GamesCheckedForUpdates { get; set; } = new List<GameType>();
        public List<Mod> ActivatedMods { get; set; } = new List<Mod>();
        public List<Mod> AllMods { get; set; } = new List<Mod>();
    }
}
