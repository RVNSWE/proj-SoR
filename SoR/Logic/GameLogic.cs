using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using SoR.Hardware.Data;
using SoR.Hardware.Graphics;
using SoR.Hardware.Input;
using SoR.Logic.GameMap.TiledScenery;
using SoR.Logic.GameMap.Interactables;
using SoR.Logic.Character;
using SoR.Logic.Character.Player;
using SoR.Logic.Character.Mobs;
using SoR.Logic.UI;
using SoR.Logic.GameMap;
using System.IO;

namespace SoR.Logic
{
    /*
     * Game logic. Manages how game elements are created, destroyed, rendered and positioned,
     * as well as handling how, when and why various elements will interact.
     */
    public partial class GameLogic
    {
        private EntityType entityType;
        private SceneryType sceneryType;
        private CurrentMap currentMapEnum;
        private MainMenu mainMenu;
        private StartMenu startMenu;
        private Map map;
        private Render render;
        private Camera camera;
        private SpriteFont font;
        private GamePadInput gamePadInput;
        private KeyboardInput keyboardInput;
        private Entity player;
        private Color backgroundColour;
        private Dictionary<string, Vector2> mapLowerWalls;
        private Dictionary<string, Vector2> mapUpperWalls;
        private Dictionary<string, Vector2> mapFloor;
        private Dictionary<string, Vector2> mapFloorDecor;
        private Dictionary<Vector2, float> tileDepths;
        private List<float> depths;
        private List<Rectangle> impassableArea;
        private string currentMenuItem;
        private bool menu;
        private bool exitMenu;
        private bool freezeGame;
        private bool loadingGame;
        private bool newGame;
        private bool hasUpperWalls;
        private bool hasFloorDecor;
        private bool fadingOut;
        private float curtainOpacity;
        private float curtainTimer;
        private int screenWidth;
        private int screenHeight;
        public Dictionary<string, Entity> Entities { get; set; }
        public Dictionary<string, Scenery> Scenery { get; set; }
        public string InGameScreen { get; set; }
        public string PlayerLocation { get; set; }
        public string SaveFile { get; set; }
        public bool FadingIn { get; set; }
        public bool CurtainUp { get; set; }
        public bool ExitGame { get; set; }

        /*
         * Differentiate between entities.
         */
        enum EntityType
        {
            Player,
            Pheasant,
            Chara,
            Slime,
            Fishy
        }

        /*
         * Differentiate between environmental ojects.
         */
        enum SceneryType
        {
            Campfire
        }

        /*
         * Constructor for initial game setup.
         */
        public GameLogic(MainGame game, GraphicsDevice GraphicsDevice)
        {
            gamePadInput = new GamePadInput();
            keyboardInput = new KeyboardInput();

            SaveFile = Globals.GetSavePath("SoR\\saveFile.json");

            InGameScreen = "mainMenu";
            hasFloorDecor = false;
            hasUpperWalls = false;
            freezeGame = false;
            loadingGame = false;
            FadingIn = false;
            CurtainUp = false;
            fadingOut = false;
            exitMenu = false;
            ExitGame = false;
            curtainOpacity = 0f;
            curtainTimer = 0f;
            backgroundColour = new Color(0, 11, 8);
            screenWidth = 0;
            screenHeight = 0;
        }

        /*
         * Save the game to a file.
         */
        public void SaveGame()
        {
            switch (currentMapEnum)
            {
                case CurrentMap.Village:
                    PlayerLocation = "village";
                    break;
                case CurrentMap.Temple:
                    PlayerLocation = "temple";
                    break;
            }
            GameState.SaveFile(player, PlayerLocation);
            PlayerLocation = "none";
        }

        /*
         * Load the game from a file once the curtain is fully opaque.
         */
        public void LoadGame(MainGame game, GameTime gameTime, GraphicsDevice GraphicsDevice)
        {
            GameState gameState = GameState.LoadFile();

            switch (gameState.CurrentMap)
            {
                case "village":
                    Village(game, GraphicsDevice);
                    break;
                case "temple":
                    Temple(game, GraphicsDevice);
                    break;
            }

            player.Position = gameState.Position;
            player.HitPoints = gameState.HitPoints;
            player.Skin = gameState.Skin;
            player.UpdateSkin(gameState.Skin);
            loadingGame = false;
        }

        /*
         * Load initial content into the game.
         */
        public void LoadGameContent(GraphicsDevice GraphicsDevice, MainGame game)
        {
            render = new Render(game, GraphicsDevice);

            // Font used for drawing text
            font = game.Content.Load<SpriteFont>("Fonts/File");
        }

        /*
         * Choose entity to create.
         */
        public void CreateEntity(GraphicsDevice GraphicsDevice, float positionX, float positionY)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    Entities.Add("player", new Player(GraphicsDevice, impassableArea) { Type = "player" });
                    if (Entities.TryGetValue("player", out Entity player))
                    {
                        player.SetPosition(positionX, positionY);
                        this.player = player;
                        player.Frozen = true;
                    }
                    break;
                case EntityType.Pheasant:
                    Entities.Add("pheasant", new Pheasant(GraphicsDevice, impassableArea) { Type = "pheasant" });
                    if (Entities.TryGetValue("pheasant", out Entity pheasant))
                    {
                        pheasant.SetPosition(positionX, positionY);
                        pheasant.Frozen = true;
                    }
                    break;
                case EntityType.Chara:
                    Entities.Add("chara", new Chara(GraphicsDevice, impassableArea) { Type = "chara" });
                    if (Entities.TryGetValue("chara", out Entity chara))
                    {
                        chara.SetPosition(positionX, positionY);
                        chara.Frozen = true;
                    }
                    break;
                case EntityType.Slime:
                    Entities.Add("slime", new Slime(GraphicsDevice, impassableArea) { Type = "slime" });
                    if (Entities.TryGetValue("slime", out Entity slime))
                    {
                        slime.SetPosition(positionX, positionY);
                        slime.Frozen = true;
                    }
                    break;
                case EntityType.Fishy:
                    Entities.Add("fishy", new Fishy(GraphicsDevice, impassableArea) { Type = "fishy" });
                    if (Entities.TryGetValue("fishy", out Entity fishy))
                    {
                        fishy.SetPosition(positionX, positionY);
                        fishy.Frozen = true;
                    }
                    break;
            }
        }

        /*
         * Choose interactable object to create.
         */
        public void CreateScenery(GraphicsDevice GraphicsDevice, float positionX, float positionY)
        {
            switch (sceneryType)
            {
                case SceneryType.Campfire:
                    Scenery.Add("campfire", new Campfire(GraphicsDevice) { Name = "campfire" });
                    if (Scenery.TryGetValue("campfire", out Scenery campfire))
                    {
                        campfire.SetPosition(positionX, positionY);
                    }
                    break;
            }
        }

        /*
         * Update world progress.
         */
        public void UpdateWorld(MainGame game, GameTime gameTime, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            camera.FollowPlayer(player.Position);

            if (!freezeGame)
            {
                foreach (var scenery in Scenery.Values)
                {
                    scenery.SetDepth(scenery.GetPosition().Y / screenHeight);
                    scenery.UpdateAnimations(gameTime);
                }

                foreach (var entity in Entities.Values)
                {
                    entity.UpdatePosition(gameTime, graphics);
                    entity.SetDepth(entity.Position.Y / screenHeight);
                    entity.UpdateAnimations(gameTime);

                    if (entity != player & player.CollidesWith(entity))
                    {
                        entity.StopMoving();

                        player.EntityCollision(entity, gameTime);
                        entity.EntityCollision(player, gameTime);
                    }
                    else if (!entity.IsMoving())
                    {
                        entity.StartMoving();
                    }

                    foreach (var scenery in Scenery.Values)
                    {
                        if (scenery.CollidesWith(entity))
                        {
                            entity.StopMoving();

                            scenery.Collision(entity, gameTime);
                        }
                        else if (!entity.IsMoving())
                        {
                            entity.StartMoving();
                        }
                    }
                }
            }
        }

        /*
         * Get the current game time.
         */
        public static float GetTime(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /*
         * Update the graphics device with the new screen resolution after a resolution change.
         */
        public void UpdateViewportGraphics(GameWindow Window, int screenWidth, int screenHeight)
        {
            camera.GetResolutionUpdate(screenWidth, screenHeight);
            camera.UpdateViewportAdapter(Window);
        }

        /*
         * Build a dictionary of relative tile depths for this map.
         */
        public void GetTileDepths()
        {
            tileDepths = [];

            foreach (var tile in mapLowerWalls.Values)
            {
                var depth = tile.Y / screenHeight;
                tileDepths.Add(tile, depth);
            }
        }

        /*
         * Get the positions of game elements before rendering.
         */
        public void RefreshPositions()
        {
            depths = [];
            foreach (var tile in tileDepths)
            {
                if (!depths.Contains(tile.Value))
                {
                    depths.Add(tile.Value);
                }
            }
            foreach (var scenery in Scenery.Values)
            {
                if (!depths.Contains(scenery.GetDepth()))
                {
                    depths.Add(scenery.GetDepth());
                }
            }
            foreach (var entity in Entities.Values)
            {
                if (!depths.Contains(entity.GetDepth()))
                {
                    depths.Add(entity.GetDepth());
                }
            }
        }

        /*
         * Render game elements in order of y-axis position.
         */
        public void Render(MainGame game, GameTime gameTime, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            GraphicsDevice.Clear(backgroundColour); // Clear the graphics buffer and set the window background colour

            switch (currentMapEnum)
            {
                case CurrentMap.MainMenu: // If current screen is MainMenu
                    currentMenuItem = render.DrawMainMenu(
                        gameTime,
                        camera.PlayerPosition.X,
                        camera.PlayerPosition.Y,
                        camera.GetCamera(),
                        camera.NewWidth,
                        camera.NewHeight,
                        render.Curtain,
                        font,
                        mainMenu.MenuOptions[0],
                        mainMenu.MenuOptions[1],
                        mainMenu.MenuOptions[2],
                        mainMenu.MenuOptions[3],
                        mainMenu.MenuOptions[4],
                        mainMenu.MenuOptions[5],
                        mainMenu.NavigateMenu(gameTime),
                        SaveFile);

                    ScreenFadeIn(gameTime, game, GraphicsDevice);
                    break;

                default: // Otherwise default to drawing game
                    foreach (var tileName in mapFloor)
                    {
                        render.StartDrawingSpriteBatch(camera.GetCamera());
                        render.DrawMap(map.GetFloorAtlas(), map, tileName.Key, tileName.Value);
                        render.FinishDrawingSpriteBatch();
                    }
                    if (hasFloorDecor)
                    {
                        foreach (var tileName in mapFloorDecor)
                        {
                            render.StartDrawingSpriteBatch(camera.GetCamera());
                            render.DrawMap(map.GetFloorDecorAtlas(), map, tileName.Key, tileName.Value);
                            render.FinishDrawingSpriteBatch();
                        }
                    }

                    // Draw elements to the screen in order of y-axis position
                    RefreshPositions();
                    depths.Sort();

                    foreach (var depth in depths)
                    {
                        render.StartDrawingSkeleton(GraphicsDevice, camera);
                        render.StartDrawingSpriteBatch(camera.GetCamera());
                        foreach (var entity in Entities.Values)
                        {
                            if (entity.GetDepth() == depth)
                            {
                                render.DrawEntitySkeleton(entity);

                                render.DrawEntitySpriteBatch(entity, font);
                            }
                        }
                        foreach (var scenery in Scenery.Values)
                        {
                            if (scenery.GetDepth() == depth)
                            {
                                render.DrawScenerySkeleton(scenery);

                                render.DrawScenerySpriteBatch(scenery, font);
                            }
                        }
                        render.FinishDrawingSkeleton();
                        render.FinishDrawingSpriteBatch();

                        foreach (var tileName in mapLowerWalls)
                        {
                            tileDepths.TryGetValue(tileName.Value, out float tileDepth);
                            if (tileName.Value.Y / screenHeight == tileDepth)
                            {
                                render.StartDrawingSpriteBatch(camera.GetCamera());
                                render.DrawMap(map.GetWallAtlas(), map, tileName.Key, tileName.Value);
                                render.FinishDrawingSpriteBatch();
                            }
                        }
                    }

                    if (hasUpperWalls)
                    {
                        foreach (var tileName in mapUpperWalls)
                        {
                            render.StartDrawingSpriteBatch(camera.GetCamera());
                            render.DrawMap(map.GetWallAtlas(), map, tileName.Key, tileName.Value);
                            render.FinishDrawingSpriteBatch();
                        }
                    }

                    if (freezeGame)
                    {
                        currentMenuItem = render.DrawStartMenu(
                            gameTime,
                            camera.PlayerPosition.X,
                            camera.PlayerPosition.Y,
                            camera.GetCamera(),
                            camera.NewWidth,
                            camera.NewHeight,
                            render.Curtain,
                            font,
                            startMenu.MenuOptions[0],
                            startMenu.MenuOptions[1],
                            startMenu.MenuOptions[2],
                            startMenu.MenuOptions[3],
                            startMenu.MenuOptions[4],
                            startMenu.MenuOptions[5],
                            startMenu.NavigateMenu(gameTime),
                            SaveFile,
                            exitMenu);
                    }

                    ScreenFadeIn(gameTime, game, GraphicsDevice);
                    ScreenCurtainHold(gameTime);
                    ScreenFadeOut(gameTime);
                    break;

            }
        }

        /*
         * Check for player input.
         */
        public void CheckInput(MainGame game, GameTime gameTime, GraphicsDevice GraphicsDevice)
        {
            string button = gamePadInput.CheckButtonInput();
            string key = keyboardInput.CheckKeyInput();
            string input = "none";

            if (button != "none")
            {
                input = button;
            }
            if (key != "none")
            {
                input = key;
            }

            if (menu) // Only applicable within menus
            {
                if (input == "A" || input == "Enter")
                {
                    switch (currentMenuItem)
                    {
                        case "Start new game":
                            FadingIn = true;
                            newGame = true;
                            ScreenFadeIn(gameTime, game, GraphicsDevice);
                            break;
                        case "Continue":
                            FadingIn = true;
                            loadingGame = true;
                            ScreenFadeIn(gameTime, game, GraphicsDevice);
                            break;
                        case "Load game":
                            FadingIn = true;
                            loadingGame = true;
                            ScreenFadeIn(gameTime, game, GraphicsDevice);
                            break;
                        case "Settings":
                            break;
                        case "Inventory":
                            break;
                        case "Exit game":
                            exitMenu = true;
                            startMenu.Select = 0;
                            startMenu.ItemCount = 1;
                            break;
                        case "Exit to main menu":
                            exitMenu = false;
                            InGameScreen = "mainMenu";
                            screenWidth = camera.NewWidth;
                            screenHeight = camera.NewHeight;
                            break;
                        case "Exit to desktop":
                            ExitGame = true;
                            break;
                    }
                }
            }
            if (!menu) // Only applicable outside of menus
            {
                if (input == "Up" || input == "F8")
                {
                    SaveGame(); // Save the current game state
                }
                if (input == "Down" || input == "F9")
                {
                    if (File.Exists(SaveFile))
                    {
                        // *** TO DO *** Change later to add "Load game?" before actually loading *** TO DO ***
                        loadingGame = true; // Game data is being loaded from file
                        FadingIn = true; // The curtain will start fading in
                        ScreenFadeIn(gameTime, game, GraphicsDevice); // Start fading in the curtain
                    }
                    else System.Diagnostics.Debug.WriteLine("No save file found."); // Otherwise write to debug console "No save file found."
                }
            }

            if (input == "Start" || input == "Escape")
            {
                switch (freezeGame) // Freeze the game whilst the menu is open
                {
                    case true: // If the menu is open
                        switch (exitMenu)
                        {
                            case true: // If in the exitMenu
                                exitMenu = false; // Leave the exitMenu
                                break;
                            case false: // Otherwise
                                InGameScreen = "game"; // Go back to the game screen
                                freezeGame = false; // Unfreeze the game
                                menu = false; // Exit the StartMenu
                                break;
                        }
                        break;
                    case false: // Otherwise
                        if (!menu)
                        {
                            InGameScreen = "startMenu"; // Switch to the StartMenu screen
                            freezeGame = true; // Freeze the game
                        }
                        break;
                }
            }
        }
    }
}