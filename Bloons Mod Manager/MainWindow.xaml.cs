using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Bloons_Mod_Manager.Lib;
using Bloons_Mod_Manager.Lib.Enums;
using Bloons_Mod_Manager.Lib.Web;
using Bloons_Mod_Manager.Wpf.Models;
using Bloons_Mod_Manager.Wpf.ViewModels;
using Bloons_Mod_Manager.Lib.Handlers;
using Bloons_Mod_Manager.Lib.Extensions;
using static Bloons_Mod_Manager.Wpf.ViewModels.ModsListViewModel;
using Bloons_Mod_Manager.Wpf.Views;
using System.IO;
using Microsoft.Win32;
using System.Windows.Documents;

namespace Bloons_Mod_Manager.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string versionNumber = "0.0.0";
        public static MainWindow instance;

        public static void RunOnMainThread(Action action) => instance.Dispatcher.Invoke(action);

        public MainWindow()
        {
            InitializeComponent();
            
            instance = this;
            DataContext = new WelcomeViewModel();

            BTD6ModsButton.DataContext = new ModButtonModel("Bloons TD6");
            BATTDModsButton.DataContext = new ModButtonModel("Bloons Adventure Time");
            BTD5ModsButton.DataContext = new ModButtonModel("Bloons TD5");
            BTDBModsButton.DataContext = new ModButtonModel("Bloons TDB");
            LaunchButton.DataContext = new ModButtonModel("Launch");

            Logger.MessageLogged += Logger_MessageLogged;
            GameSelected += ModsListViewModel_GameSelected;
            Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            ModsListView.OnWindowFocused();
        }

        // Used as a "Finished Loading" event
        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await IsModManagerUpdates();
        }

        bool IsGameInstalled(GameType gameType)
        {
            if (!gameType.IsInstalled())
            {
                Logger.Log("This game is not installed. Please install it to continue", OutputType.ConsoleAndMsgBox);
                return false;
            }

            return true;
        }

        private async void ModsListViewModel_GameSelected(object sender, GameSelectedEventArgs e)
        {
            var gameInfo = e.Game.GetGameInfo();
            Directory.CreateDirectory(gameInfo.ModsDir);
            Directory.CreateDirectory(gameInfo.UnusedModsDir);

            if (e.Game.IsMelonloaderGame())
            {
                if (SessionData.instance.GamesCheckedForUpdates.Contains(e.Game))
                    return;
                await CheckForMelonUpdates(e.Game);
                await CheckForModHelperUpdates(e.Game);

                SessionData.instance.GamesCheckedForUpdates.Add(e.Game);
            }
        }

        private async Task IsModManagerUpdates()
        {
            try
            {
                const string url = "https://api.github.com/repos/gurrenm3/Bloons-Mod-Manager/releases";

                var updater = new UpdateChecker(url);
                var releaseInfo = await updater.GetReleaseInfoAsync(url);
                var latestRelease = releaseInfo[0];
                bool isUpdate = updater.IsUpdate(versionNumber, latestRelease);
                if (isUpdate)
                {
                    var msgBoxResult = MessageBox.Show("An update is available for the Mod Manager. Do you want to download it now?",
                        "An update is available", MessageBoxButton.YesNo);
                    if (msgBoxResult == MessageBoxResult.Yes)
                        Process.Start("https://github.com/gurrenm3/Bloons-Mod-Manager/releases");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }

        private async Task CheckForMelonUpdates(GameType game)
        {
            try
            {
                var melonHandler = new MelonLoaderHandler(game);
                bool isUpdate = await melonHandler.IsUpdateAvailableAsync();
                if (isUpdate)
                {
                    var msgBoxResult = MessageBox.Show("An update is available for MelonLoader. You need it in order to play mods. Do you want to download it now?",
                        "An update is available", MessageBoxButton.YesNo);
                    if (msgBoxResult == MessageBoxResult.Yes)
                        Process.Start("https://github.com/LavaGang/MelonLoader/releases");
                }
            }
            catch (Exception) {  }
        }

        private async Task CheckForModHelperUpdates(GameType game)
        {
            try
            {
                bool isUpdate = await new ModHelperHandler(game).IsUpdateAsync();
                if (isUpdate)
                {
                    var msgBoxResult = MessageBox.Show("An update is available for BTD Mod Helper. Do you want to download it now?",
                        "An update is available", MessageBoxButton.YesNo);
                    if (msgBoxResult == MessageBoxResult.Yes)
                        Process.Start("https://github.com/gurrenm3/BTD-Mod-Helper/releases");
                }
                else
                {
                    Logger.Log("Mod Helper is up to date");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }


        #region UI Events

        private void AddModsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SessionData.instance.CurrentGame == GameType.None)
            {
                string msg = "You need to select a game before continuing";
                Logger.Log(msg, OutputType.Console);
                MessageBox.Show(msg, "Select a game");
                return;
            }

            AddMods();
        }

        private void AddMods()
        {
            var files = OpenModsFileDialog();
            if (files is null)
                return;

            bool modsWereAdded = false;
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (SessionData.instance.CurrentGame.IsMelonloaderGame() && !fileInfo.IsMelonMod())
                {
                    Logger.Log($"Can't load \"{file}\" because it's not a MelonMod", OutputType.Console);
                    continue;
                }

                string newPath = SessionData.instance.CurrentGame.GetGameInfo().ModsDir + "\\" + fileInfo.Name;
                File.Copy(file, newPath, overwrite: true);
                modsWereAdded = true;
            }

            if (modsWereAdded)
                ModsListView.instance?.TryPopulateMods();
        }

        private string[] OpenModsFileDialog()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Browse for mods";
            dialog.Filter = "DLLs (*.dll) | *dll";
            dialog.InitialDirectory = SessionData.instance.CurrentGame != GameType.None
                ? SessionData.instance.CurrentGame.GetGameInfo().GameDir : Environment.CurrentDirectory;

            dialog.Multiselect = true;
            var selectedFiles = dialog.ShowDialog();
            return selectedFiles.HasValue ? dialog.FileNames : null;
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            var launcher = new Launcher(SessionData.instance.CurrentGame, SessionData.instance.CurrentPlatform);
            launcher.MoveUnusedMods();
            launcher.Launch();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ModsButtonScrollView.ActualWidth - 35 <= 0)
                return;

            BTD6ModsButton.Width = ModsButtonScrollView.ActualWidth - 15;
            BATTDModsButton.Width = ModsButtonScrollView.ActualWidth - 15;
            BTD5ModsButton.Width = ModsButtonScrollView.ActualWidth - 15;
            BTDBModsButton.Width = ModsButtonScrollView.ActualWidth - 15;
            LaunchButton.Width = ModsButtonScrollView.ActualWidth - 35;
        }

        private void BTD6ModsButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameInstalled(GameType.BloonsTD6))
                DataContext = new ModsListViewModel(GameType.BloonsTD6);
        }

        private void BATTDModsButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameInstalled(GameType.BloonsAdventureTime))
                DataContext = new ModsListViewModel(GameType.BloonsAdventureTime);
        }

        private void BTD5ModsButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameInstalled(GameType.BloonsTD5))
                DataContext = new ModsListViewModel(GameType.BloonsTD5);
        }

        private void BTDBModsButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameInstalled(GameType.BloonsTDBattles))
                DataContext = new ModsListViewModel(GameType.BloonsTDBattles);
        }

        private void Logger_MessageLogged(object sender, Logger.LogEvents e)
        {
            Dispatcher.Invoke(() => {
                if (e.Output == OutputType.Console || e.Output == OutputType.ConsoleAndMsgBox)
                {

                }

                if (e.Output == OutputType.MsgBox || e.Output == OutputType.ConsoleAndMsgBox)
                    MessageBox.Show(e.Message);

                Debug.WriteLine(e.Message);
            });
        }

        #endregion

       
    }
}
