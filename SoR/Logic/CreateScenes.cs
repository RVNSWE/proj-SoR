using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SoR.Logic.UI;
using SoR.Logic.GameMap.TiledScenery;
using SoR.Hardware.Graphics;

namespace SoR.Logic
{
    /*
     * Create and position the elements of each map area, and remove them on scene transition.
     */
    public partial class GameLogic
    {
        /*
         * The game map currently in use.
         */
        enum CurrentMap
        {
            MainMenu,
            Village,
            Temple
        }

        /*
         * Fade in the curtain.
         */
        public void ScreenFadeIn(GameTime gameTime, MainGame game, GraphicsDevice GraphicsDevice)
        {
            if (FadingIn)
            {
                float deltaTime = GetTime(gameTime);
                float timeLength = 0.3f; // For 0.3 of a second
                curtainOpacity += deltaTime * 3.33f; // Increment the curtain opacity

                if (curtainTimer < timeLength) // If the curtainTimer hasn't reached the total timeLength
                {
                    curtainTimer += deltaTime; // Increment the curtainTimer

                    if (curtainOpacity > 1f)
                    {
                        curtainOpacity = 1f; // Make sure fadeAlpha is never more than 1f (fully opaque)
                    }

                    render.DrawCurtain(
                        camera.PlayerPosition.X,
                        camera.PlayerPosition.Y,
                        camera.GetCamera(),
                        camera.NewWidth,
                        camera.NewHeight,
                        render.Curtain,
                        curtainOpacity); // Draw the curtain with the current opacity
                }

                if (curtainTimer >= timeLength) // If the curtainTimer has reached or exceeded timeLength
                {
                    render.DrawCurtain(
                        camera.PlayerPosition.X,
                        camera.PlayerPosition.Y,
                        camera.GetCamera(),
                        camera.NewWidth,
                        camera.NewHeight,
                        render.Curtain); // Draw the curtain at full opacity

                    if (newGame) // If starting a new game
                    {
                        Village(game, GraphicsDevice); // Load the starting area
                    }
                    if (loadingGame) // If loading from save file
                    {
                        LoadGame(game, gameTime, GraphicsDevice); // Load the save data
                    }

                    curtainOpacity = 1f;
                    curtainTimer = 0f;
                    freezeGame = false;
                    FadingIn = false;
                    CurtainUp = true; // Reset variables, unfreeze game, curtain is now up
                }
            }
        }

        /*
         * Hold the curtain in place.
         */
        public void ScreenCurtainHold(GameTime gameTime)
        {
            if (CurtainUp)
            {
                float deltaTime = GetTime(gameTime);
                float timeLength = 0.5f; // For half a second
                curtainTimer += deltaTime; // Increment the curtainTimer

                render.DrawCurtain(
                    camera.PlayerPosition.X,
                    camera.PlayerPosition.Y,
                    camera.GetCamera(),
                    camera.NewWidth,
                    camera.NewHeight,
                    render.Curtain); // Draw the curtain

                if (curtainTimer >= timeLength) // If the max curtainTime has reached or exceeded timeLength
                {
                    curtainTimer = 0f;
                    CurtainUp = false;
                    fadingOut = true; // Reset variables, curtain is now fadingOut
                }
            }
        }

        /*
         * Fade out the curtain.
         */
        public void ScreenFadeOut(GameTime gameTime)
        {
            if (fadingOut)
            {
                float deltaTime = GetTime(gameTime);
                float timeLength = 1f; // For 1 second
                curtainTimer += deltaTime; // Increment the curtainTimer
                curtainOpacity -= deltaTime; // Decrement the opacity

                if (curtainTimer < timeLength) // If curtainTimer is less than total timeLength
                {
                    if (curtainOpacity < 0f)
                    {
                        curtainOpacity = 0f; // Ensure opacity is never less than 0f
                    }

                    render.DrawCurtain(
                        camera.PlayerPosition.X,
                        camera.PlayerPosition.Y,
                        camera.GetCamera(),
                        camera.NewWidth,
                        camera.NewHeight,
                        render.Curtain,
                        curtainOpacity); // Draw curtain at current opacity
                }

                if (curtainTimer >= timeLength) // If the max curtainTime has reached or exceeded timeLength
                {
                    curtainOpacity = 0f;
                    curtainTimer = 0f;
                    fadingOut = false; // Reset variables, the curtain is finished with
                }
            }
        }

        /*
         * Set up the main game menu.
         */
        public void GameMainMenu(MainGame game, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            menu = true;
            Entities = [];
            Scenery = [];
            mapLowerWalls = [];
            mapUpperWalls = [];
            mapFloor = [];
            mapFloorDecor = [];
            depths = [];
            impassableArea = [];
            InGameScreen = "none";
            PlayerLocation = "none";
            currentMenuItem = "none";
            camera = new Camera(game.Window, GraphicsDevice, 800, 600);
            camera.UpdateViewportAdapter(game.Window);
            camera.NewWidth = screenWidth;
            camera.NewHeight = screenHeight;
            mainMenu = new MainMenu(game, graphics);
            mainMenu.ItemCount = 4; // Reset the number of StartMenu items to 4
            currentMapEnum = CurrentMap.MainMenu;
            LoadGameContent(GraphicsDevice, game);
        }

        /*
         * Set up the main game menu.
         */
        public void GameStartMenu(MainGame game, GraphicsDevice GraphicsDevice)
        {
            menu = true;
            InGameScreen = "game";
            startMenu = new StartMenu(game, GraphicsDevice);
            startMenu.ItemCount = 3; // Reset the number of StartMenu items to 3
            LoadGameContent(GraphicsDevice, game);
        }

        /*
         * Create the Village scene.
         */
        public void Village(MainGame game, GraphicsDevice GraphicsDevice)
        {
            menu = false;
            newGame = false;
            InGameScreen = "game";

            // Get the map to be used
            map = new Map(1);
            currentMapEnum = CurrentMap.Village;
            LoadGameContent(GraphicsDevice, game);
            hasFloorDecor = true;
            hasUpperWalls = false;

            // Create the map
            map.LoadMap(game.Content, map.FloorSpriteSheet, map.WallSpriteSheet, map.FloorDecorSpriteSheet);
            mapLowerWalls = render.CreateMap(map, map.LowerWalls, true);
            mapUpperWalls = [];
            mapFloor = render.CreateMap(map, map.Floor);
            mapFloorDecor = render.CreateMap(map, map.FloorDecor);
            render.ImpassableMapArea();
            impassableArea = render.ImpassableTiles;
            GetTileDepths();

            // Re-initialise the entity and scenery arrays
            Entities = [];
            Scenery = [];

            // Create entities
            entityType = EntityType.Player;
            CreateEntity(GraphicsDevice, 250, 200);

            entityType = EntityType.Chara;
            CreateEntity(GraphicsDevice, 200, 250);

            entityType = EntityType.Pheasant;
            CreateEntity(GraphicsDevice, 250, 250);
        }

        /*
         * Create the Temple scene.
         */
        public void Temple(MainGame game, GraphicsDevice GraphicsDevice)
        {
            menu = false;
            InGameScreen = "game";

            // Get the map to be used
            map = new Map(0);
            currentMapEnum = CurrentMap.Temple;
            LoadGameContent(GraphicsDevice, game);
            hasFloorDecor = false;
            hasUpperWalls = true;

            // Create the map
            map.LoadMap(game.Content, map.FloorSpriteSheet, map.WallSpriteSheet);
            mapLowerWalls = render.CreateMap(map, map.LowerWalls, true);
            mapUpperWalls = render.CreateMap(map, map.UpperWalls);
            mapFloor = render.CreateMap(map, map.Floor);
            mapFloorDecor = [];
            render.ImpassableMapArea();
            impassableArea = render.ImpassableTiles;
            GetTileDepths();

            // Re-initialise the entity and scenery arrays
            Entities = [];
            Scenery = [];

            // Create entities
            entityType = EntityType.Player;
            CreateEntity(GraphicsDevice, 220, 100);

            entityType = EntityType.Fishy;
            CreateEntity(GraphicsDevice, 280, 180);

            entityType = EntityType.Slime;
            CreateEntity(GraphicsDevice, 250, 200);

            // Create scenery
            sceneryType = SceneryType.Campfire;
            CreateScenery(GraphicsDevice, 224, 160);
        }
    }
}