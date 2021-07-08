using Bloons_Mod_Manager.Lib;
using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Wpf.Views;
using System;

namespace Bloons_Mod_Manager.Wpf.ViewModels
{
    class ModsListViewModel
    {
        public static event EventHandler<GameSelectedEventArgs> GameSelected;

        public ModsListViewModel(GameType gameType)
        {
            MainWindow.instance.AddModsButton.Visibility = System.Windows.Visibility.Visible;
            SessionData.instance.CurrentGame = gameType;
            OnGameSelected(gameType);
        }

        public void OnGameSelected(GameType gameType)
        {
            var args = new GameSelectedEventArgs();
            args.Game = gameType;
            GameSelected?.Invoke(this, args);
        }

        public class GameSelectedEventArgs : EventArgs
        {
            public GameType Game { get; set; }
        }
    }
}
