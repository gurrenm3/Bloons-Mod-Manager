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

            TryPopulateMods();
            ModsContainer.SelectionChanged += ModsContainer_SelectionChanged;
        }

        private void ModsListViewModel_GameSelected(object sender, ModsListViewModel.GameSelectedEventArgs e)
        {
            _game = SessionData.instance.CurrentGame;
            _gameInfo = _game.GetGameInfo();
            instance?.TryPopulateMods();
        }

        public bool TryPopulateMods()
        {
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

            return true;
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
            instance?.TryPopulateMods();
        }
    }
}
