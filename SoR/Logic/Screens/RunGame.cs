using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SoR.Logic.Screens
{ /*
   * Load, update and draw screens.
   */
    public partial class Screens
    {
        /*
         * Starts the game on the main game menu.
         */
        public void LoadGame(MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            gameLogic.GameMainMenu(game, GraphicsDevice);
        }

        /*
         * Update the resolution after a screen size change.
         */
        public void UpdateResolution(GameWindow Window, int screenWidth, int screenHeight)
        {
            gameLogic.UpdateViewportGraphics(Window, screenWidth, screenHeight);
        }

        /*
         * Draw the current state of the game to the screen.
         */
        public void DrawGame(MainGame game, GameTime gameTime, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            gameLogic.Render(game, gameTime, GraphicsDevice, graphics);
        }
    }
}