namespace SoR.Logic.UI
{
    internal class MainMenu : Menu
    {
        /*
         * Set up the main menu.
         */

        public MainMenu(MainGame game)
        {
            InitialiseInput(game);
            InitialiseMenu(game);

            MenuOptions = ["Game Title", "Start new game", "Continue", "Load game", "Settings", "Exit to desktop"];
        }
    }
}