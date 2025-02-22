using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.IO;

namespace SoR.Hardware.Graphics
{
    internal partial class Render
    {

        /*
         * Draw the MainMenu.
         * 
         * *** TO DO *** Reposition *** TO DO ***
         */
        public string DrawMainMenu(
            GameTime gameTime,
            float positionX,
            float positionY,
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
            Vector2 backgroundPosition = new Vector2(positionX, positionY);
            Vector2 titlePosition = new Vector2(positionX - 125, positionY - 156);
            Vector2 newGamePosition = new Vector2(positionX - 250, positionY + 20);
            Vector2 continueGamePosition = new Vector2(positionX - 250, positionY + 50);
            Vector2 loadGamePosition = new Vector2(positionX - 250, positionY + 80);
            Vector2 gameSettingsPosition = new Vector2(positionX - 250, positionY + 110);
            Vector2 toDesktopPosition = new Vector2(positionX - 250, positionY + 140);

            DrawCurtain(positionX, positionY, camera, width, height);

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
            float positionX,
            float positionY,
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
            Vector2 backgroundPosition = new Vector2(positionX, positionY);
            Vector2 inventoryPosition = new Vector2(positionX - 350, positionY - 156);
            Vector2 gameSettingsPosition = new Vector2(positionX - 350, positionY - 56);
            Vector2 loadGamePosition = new Vector2(positionX - 350, positionY + 44);
            Vector2 exitGamePosition = new Vector2(positionX - 350, positionY + 144);
            Vector2 toMainMenuPosition = new Vector2(positionX - 50, positionY - 56);
            Vector2 toDesktopPosition = new Vector2(positionX - 50, positionY + 44);

            DrawCurtain(positionX, positionY, camera, width, height, 0.75f);

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
                DrawCurtain(positionX, positionY, camera, width, height, 0.15f);

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
