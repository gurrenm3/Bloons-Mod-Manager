namespace Bloons_Mod_Manager.Wpf.Models
{
    class ModButtonModel
    {
        public string Name { get; set; }
        public int MaxWidth { get; set; } = 200;
        public int MaxHeight { get; set; } = 100;
        public int Height { get; set; } = 50;
       

        public ModButtonModel(string name)
        {
            Name = name;
        }
    }
}
