using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.IO;

namespace SoR.Hardware.Graphics
{
    internal partial class Render
    {

        /*
         * Draw the MainMenu.
         */
        public string DrawMainMenu(
            GameTime gameTime,
            Vector2 position,
            OrthographicCamera camera,
            int width,
            int height,
            string title,
            string newGame,
            string continueGame,
            string loadGame,
            string settings,
            string exitGame,
            int select,
            string saveFile)
        {
            string currentMenuItem = "none";

            // Use camera.PlayerPosition as point of reference for positioning since it's updated with screen resolution
            Vector2 backgroundPosition = new Vector2(position.X, position.Y);
            Vector2 titlePosition = new Vector2(position.X - 125, position.Y - 156);
            Vector2 newGamePosition = new Vector2(position.X - 250, position.Y + 20);
            Vector2 continueGamePosition = new Vector2(position.X - 250, position.Y + 50);
            Vector2 loadGamePosition = new Vector2(position.X - 250, position.Y + 80);
            Vector2 gameSettingsPosition = new Vector2(position.X - 250, position.Y + 110);
            Vector2 toDesktopPosition = new Vector2(position.X - 250, position.Y + 140);

            DrawCurtain(position, camera, width, height);

            StartDrawingSpriteBatch(camera);
            MenuText(title, titlePosition, Color.GhostWhite, 2.5f);
            MenuText(newGame, newGamePosition, Color.Gray, 1);
            MenuText(continueGame, continueGamePosition, Color.Gray, 1);
            MenuText(loadGame, loadGamePosition, Color.Gray, 1);
            MenuText(settings, gameSettingsPosition, Color.Gray, 1);
            MenuText(exitGame, toDesktopPosition, Color.Gray, 1);
            FinishDrawingSpriteBatch();

            switch (select)
            {
                case 0:
                    StartDrawingSpriteBatch(camera);
                    MenuText(newGame, newGamePosition, Color.GhostWhite, 1);
                    FinishDrawingSpriteBatch();
                    currentMenuItem = newGame;
                    break;
                case 1:
                    if (File.Exists(saveFile))
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(continueGame, continueGamePosition, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = continueGame;
                    }
                    else
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(continueGame, continueGamePosition, Color.LightCoral, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = "none";
                    }
                    break;
                case 2:
                    if (File.Exists(saveFile))
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(loadGame, loadGamePosition, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = loadGame;
                    }
                    else
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(loadGame, loadGamePosition, Color.LightCoral, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = "none";
                    }
                    break;
                case 3:
                    StartDrawingSpriteBatch(camera);
                    MenuText(settings, gameSettingsPosition, Color.GhostWhite, 1);
                    FinishDrawingSpriteBatch();
                    currentMenuItem = settings;
                    break;
                case 4:
                    StartDrawingSpriteBatch(camera);
                    MenuText(exitGame, toDesktopPosition, Color.GhostWhite, 1);
                    FinishDrawingSpriteBatch();
                    currentMenuItem = exitGame;
                    break;
            }

            return currentMenuItem;
        }

        /*
         * Draw the StartMenu.
         */
        public string DrawStartMenu(
            GameTime gameTime,
            Vector2 position,
            OrthographicCamera camera,
            int width,
            int height,
            string inventory,
            string settings,
            string loadGame,
            string exitGame,
            string toMain,
            string toDesktop,
            int select,
            string saveFile,
            bool exitMenu)
        {
            string currentMenuItem = "none";

            // Use camera.PlayerPosition as point of reference for positioning since it's updated with screen resolution
            Vector2 backgroundPosition = new Vector2(position.X, position.Y);
            Vector2 inventoryPosition = new Vector2(position.X - 350, position.Y - 156);
            Vector2 gameSettingsPosition = new Vector2(position.X - 350, position.Y - 56);
            Vector2 loadGamePosition = new Vector2(position.X - 350, position.Y + 44);
            Vector2 exitGamePosition = new Vector2(position.X - 350, position.Y + 144);
            Vector2 toMainMenuPosition = new Vector2(position.X - 50, position.Y - 56);
            Vector2 toDesktopPosition = new Vector2(position.X - 50, position.Y + 44);

            DrawCurtain(position, camera, width, height, 0.75f);

            StartDrawingSpriteBatch(camera);
            MenuText(inventory, inventoryPosition, Color.Gray, 1);
            MenuText(settings, gameSettingsPosition, Color.Gray, 1);
            MenuText(loadGame, loadGamePosition, Color.Gray, 1);
            MenuText(exitGame, exitGamePosition, Color.Gray, 1);
            FinishDrawingSpriteBatch();

            if (!exitMenu)
            {
                switch (select)
                {
                    case 0:
                        StartDrawingSpriteBatch(camera);
                        MenuText(inventory, inventoryPosition, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = inventory;
                        break;
                    case 1:
                        StartDrawingSpriteBatch(camera);
                        MenuText(settings, gameSettingsPosition, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = settings;
                        break;
                    case 2:
                        if (File.Exists(saveFile))
                        {
                            StartDrawingSpriteBatch(camera);
                            MenuText(loadGame, loadGamePosition, Color.GhostWhite, 1);
                            FinishDrawingSpriteBatch();
                            currentMenuItem = loadGame;
                        }
                        else
                        {
                            StartDrawingSpriteBatch(camera);
                            MenuText(loadGame, loadGamePosition, Color.LightCoral, 1);
                            FinishDrawingSpriteBatch();
                            currentMenuItem = "none";
                        }
                        break;
                    case 3:
                        StartDrawingSpriteBatch(camera);
                        MenuText(exitGame, exitGamePosition, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = exitGame;
                        break;
                }
            }
            else
            {
                DrawCurtain(position, camera, width, height, 0.15f);

                StartDrawingSpriteBatch(camera);
                MenuText(toMain, toMainMenuPosition, Color.Gray, 1.25f);
                MenuText(toDesktop, toDesktopPosition, Color.Gray, 1.25f);
                FinishDrawingSpriteBatch();

                switch (select)
                {
                    case 0:
                        StartDrawingSpriteBatch(camera);
                        MenuText(toMain, toMainMenuPosition, Color.GhostWhite, 1.25f);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = toMain;
                        break;
                    case 1:
                        StartDrawingSpriteBatch(camera);
                        MenuText(toDesktop, toDesktopPosition, Color.GhostWhite, 1.25f);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = toDesktop;
                        break;
                }
            }

            return currentMenuItem;
        }
    }
}
