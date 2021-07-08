using Bloons_Mod_Manager.Lib;
using MaterialDesignThemes.Wpf;
using System.Linq;
using System.Windows;

namespace Bloons_Mod_Manager.Wpf.Extensions
{
    public static class CardExtensions
    {
        public static Card CreateFromMod(this Card card, Mod mod)
        {
            card.Content = mod.Name;

            if (mod.IsOlderMelonMod)
                card.Content += " (MelonLoader 2.7.4 and below)";

            card.Padding = new Thickness(16);
            card.Margin = new Thickness(8);
            card.Width = MainWindow.instance.ContentController.ActualWidth - 50;

            return card;
        }

        public static Mod GetModFromContent(this Card card)
        {
            return SessionData.instance.AllMods.FirstOrDefault(mod => card.Content.ToString().Contains(mod.Name));
        }
    }
}
