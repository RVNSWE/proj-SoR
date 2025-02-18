namespace SoR.Logic.UI
{
    internal class ChooseName : Menu
    {
        /*
         * Choose a character name.
         */

        public ChooseName(MainGame game)
        {
            InitialiseInput(game);
            InitialiseMenu(game);

            MenuOptions = ["Accept"];
        }
    }
}