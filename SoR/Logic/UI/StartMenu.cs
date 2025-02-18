namespace SoR.Logic.UI
{
    /*
     * Set up the start menu.
     */
    internal class StartMenu : Menu
    {
        public StartMenu(MainGame game)
        {
            InitialiseInput(game);
            InitialiseMenu(game);

            MenuOptions = ["Inventory", "Settings", "Load game", "Exit game", "Exit to main menu", "Exit to desktop"];
        }
    }
}