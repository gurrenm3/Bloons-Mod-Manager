using Bloons_Mod_Manager.Lib;
using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.Extensions;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bloons_Mod_Manager.Wpf.Extensions;
using System.Windows.Media;
using System.Collections.Generic;
using Bloons_Mod_Manager.Wpf.ViewModels;
using System;
using Microsoft.Win32;

namespace Bloons_Mod_Manager.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ModsListView.xaml
    /// </summary>
    public partial class ModsListView : UserControl
    {
        internal static ModsListView instance;
        private readonly Brush defaultCardBackground = Brushes.White;
        private readonly Brush activatedCardBackground = Brushes.MediumPurple;
        private GameType _game;
        private GameInfo _gameInfo;

        public ModsListView()
        {
            InitializeComponent();
            instance = this;
            _game = SessionData.instance.CurrentGame;
            _gameInfo = _game.GetGameInfo();

            ModsListViewModel.GameSelected += ModsListViewModel_GameSelected;
            ModsContainer.SelectionChanged += ModsContainer_SelectionChanged;

            Loaded += ModsListView_Loaded;
        }

        private void ModsListView_Loaded(object sender, RoutedEventArgs e)
        {
            TryPopulateMods();
        }

        private void ModsListViewModel_GameSelected(object sender, ModsListViewModel.GameSelectedEventArgs e)
        {
            _game = SessionData.instance.CurrentGame;
            _gameInfo = _game.GetGameInfo();
            TryPopulateMods();
        }

        public bool TryPopulateMods()
        {
            // check if game dir was set
            if (string.IsNullOrEmpty(_gameInfo.GameDir) || !Directory.Exists(_gameInfo.GameDir))
            {
                bool foundGameDir = BrowseForGameDir(out string gameDir);
                if (!foundGameDir)
                    return false;

                switch (_gameInfo.Game)
                {
                    case GameType.BloonsTD6:
                        UserData.Instance.BTD6Dir = gameDir;
                        break;
                    case GameType.BloonsAdventureTime:
                        UserData.Instance.BTDATDir = gameDir;
                        break;
                }

                _gameInfo.GameDir = gameDir;
                _gameInfo.ModsDir = $"{gameDir}\\Mods";
                _gameInfo.UnusedModsDir = $"{gameDir}\\Mods\\Unused Mods";

                Directory.CreateDirectory(_gameInfo.ModsDir);
                Directory.CreateDirectory(_gameInfo.UnusedModsDir);
                UserData.SaveUserData();
            }

            List<Mod> previousActiveMods = new List<Mod>();
            previousActiveMods.AddRange(SessionData.instance.ActivatedMods);

            ModsContainer.Items.Clear();
            SessionData.instance.AllMods.Clear();
            SessionData.instance.ActivatedMods.Clear();

            if (_game.IsMelonloaderGame())
            {
                var melonMods = _gameInfo.GetAllMelonMods();

                foreach (var melonMod in melonMods)
                {
                    var mod = new Mod(_game, melonMod);
                    bool previouslyActive = previousActiveMods.Any(m => m.Name == mod.Name);

                    Card card = new Card().CreateFromMod(mod);
                    MainWindow.instance.SizeChanged += MainWindow_SizeChanged;
                    card.Background = previouslyActive ? activatedCardBackground : defaultCardBackground;
                    card.Foreground = previouslyActive ? Brushes.White : Brushes.Black;
                    
                    ModsContainer.Items.Add(card);
                    SessionData.instance.AllMods.Add(mod);
                    if (previouslyActive)
                        mod.SetActive(true);
                }
            } 
            
            instance.noModsText.Visibility = (ModsContainer.Items.Count == 0) ? Visibility.Visible : Visibility.Hidden;

            return true;
        }

        private bool BrowseForGameDir(out string gameDir)
        {
            gameDir = null;
            while (true)
            {
                Logger.Log($"Game directory for {_gameInfo.Game.ToString()} was not set. Please browse for {_gameInfo.EXEName}", OutputType.ConsoleAndMsgBox);

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = $"Browse for {_gameInfo.EXEName}";
                ofd.DefaultExt = "exe";
                ofd.Filter = "Exe files (*exe)|*.exe";
                ofd.Multiselect = false;
                ofd.CheckFileExists = true;

                if (!ofd.ShowDialog().Value || !ofd.FileName.EndsWith(_gameInfo.EXEName))
                {
                    string message = $"You did not select the correct file, you need to select {_gameInfo.EXEName}. Would you like to try again?";
                    Logger.Log(message, OutputType.Console);
                    var result = MessageBox.Show(message, "Try Again?", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                        continue;

                    Logger.Log($"You did not set the game directory. You will be unable to use mods for {_gameInfo.Game} until you do so.");
                    return false;
                }
                else
                {
                    gameDir = new FileInfo(ofd.FileName).Directory.FullName;
                    return true;
                }
            }
        }

        private void ModsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            const int invalidIndex = -1;
            if (ModsContainer.SelectedIndex == invalidIndex)
                return; 

            var card = (Card)ModsContainer.SelectedItem;
            var mod = card.GetModFromContent();
            if (mod is null)
            {
                Logger.Log("Failed to get selected mod.");
                return;
            }

            bool isActive = mod.IsActivated();
            card.Background = isActive ? defaultCardBackground : activatedCardBackground;
            card.Foreground = isActive ? Brushes.Black : Brushes.White;
            mod.SetActive(!isActive);
            ModsContainer.SelectedIndex = invalidIndex;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            for (int i = 0; i < ModsContainer.Items.Count; i++)
            {
                ((Card)ModsContainer.Items[i]).Width = MainWindow.instance.ContentController.ActualWidth - 50;
            }
        }

        internal static void OnWindowFocused()
        {
            //instance?.TryPopulateMods();
        }
    }
}
