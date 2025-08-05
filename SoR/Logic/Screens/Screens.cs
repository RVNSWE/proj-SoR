using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoR.Logic.Character;

namespace SoR.Logic.Screens
{
    /*
     * Tell GameLogic which aspects of the game it should currently be rendering and/or utilising
     * depending on the current game state. Probably should stick it all back in GameLogic,
     * since... it only calls things from GameLogic. But. Everyone else seemed to have a separate
     * class for this stuff. So I just copied. I will un-copy later, probably.
     */
    public class Screens
    {
        private GameLogic gameLogic;
        public bool ExitGame { get; set; }

        public Screens(MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            gameLogic = new GameLogic(game, GraphicsDevice);
            ExitGame = false;
        }
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

        /*
         * Update the state of the game.
         */
        public void UpdateGameState(GameTime gameTime, MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            gameLogic.CheckInput(game, gameTime, GraphicsDevice);
            ExitGame = gameLogic.ExitGame;

            switch (gameLogic.InGameScreen)
            {
                case "mainMenu":
                    gameLogic.GameMainMenu(game, GraphicsDevice);
                    break;
                case "startMenu":
                    gameLogic.GameStartMenu(game, GraphicsDevice);
                    break;
                case "startNewGame":
                    gameLogic.StartNewGame(game, GraphicsDevice);
                    break;
                case "game":
                    gameLogic.UpdateWorld(game, gameTime, GraphicsDevice, graphics);
                    /*foreach (var entity in gameLogic.Entities.Values)
                    {
                        if (gameLogic.Entities.TryGetValue("chara", out Entity chara))
                        {
                            if (chara.GetHitPoints() <= 98)
                            {
                                gameLogic.FadingIn = true;
                                if (gameLogic.CurtainUp)
                                {
                                    gameLogic.Temple(game, GraphicsDevice);
                                }
                            }
                        }
                    }*/
                    break;
            }
        }
    }
}