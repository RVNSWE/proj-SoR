using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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
using SoR.Gameplay.Intro;

namespace SoR.Logic
{
    /*
     * Game logic. Manages how game elements are created, destroyed, rendered and positioned,
     * as well as handling how, when and why various elements will interact.
     * 
     * This has become a monster. If it doesn't get split out into smaller classes, it'll at
     * least get some more partial classes for the sake of organisation and personal sanity.
     */
    public partial class GameLogic
    {
        private EntityType entityType;
        private SceneryType sceneryType;
        private CurrentMap currentMapEnum;
        private Color backgroundColour;
        private MainMenu mainMenu;
        private StartMenu startMenu;
        private ChooseName chooseName;
        private Map map;
        private Render render;
        private Camera camera;
        private GamePadInput gamePadInput;
        private KeyboardInput keyboardInput;
        private Entity player;
        private Text text;
        private Dictionary<string, Vector2> mapLowerWalls;
        private Dictionary<string, Vector2> mapUpperWalls;
        private Dictionary<string, Vector2> mapFloor;
        private Dictionary<string, Vector2> mapFloorDecor;
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
        private string playerName;
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
            playerName = "Mercura";
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
                case CurrentMap.Wall:
                    PlayerLocation = "wall";
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
                case "wall":
                    Wall(game, GraphicsDevice);
                    break;
            }

            player.SetPosition(gameState.Position.X, gameState.Position.Y);
            player.HitPoints = gameState.HitPoints;
            player.UpdateSkin(gameState.Skin);
            playerName = player.Name = gameState.Name;

            loadingGame = false;
        }

        /*
         * Load initial content into the game.
         */
        public void LoadGameContent(GraphicsDevice GraphicsDevice, MainGame game)
        {
            render = new Render(game, GraphicsDevice);
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
            camera.FollowPlayer(player.GetPosition());

            if (!freezeGame)
            {
                foreach (var scenery in Scenery.Values)
                {
                    scenery.UpdateAnimations(gameTime);
                }

                foreach (var entity in Entities.Values)
                {
                    entity.UpdatePosition(gameTime, graphics);
                    entity.UpdateAnimations(gameTime);

                    if (entity != player & player.CollidesWith(entity))
                    {
                        entity.PauseMoving(gameTime);

                        player.EntityCollision(entity, gameTime);
                        entity.EntityCollision(player, gameTime);
                    }

                    foreach (var scenery in Scenery.Values)
                    {
                        if (scenery.CollidesWith(entity))
                        {
                            if (entity != player)
                            {
                                entity.PauseMoving(gameTime);
                            }
                            entity.SceneryCollision(scenery, gameTime);
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
         * Get the positions of game elements before rendering.
         */
        public void RefreshDepths()
        {
            depths = [];
            foreach (var scenery in Scenery.Values)
            {
                if (!depths.Contains(scenery.GetPosition().Y))
                {
                    depths.Add(scenery.GetPosition().Y);
                }
            }
            foreach (var entity in Entities.Values)
            {
                if (!depths.Contains(entity.GetPosition().Y))
                {
                    depths.Add(entity.GetPosition().Y);
                }
            }
            foreach (var tile in mapLowerWalls.Values)
            {
                if (!depths.Contains(tile.Y))
                {
                    depths.Add(tile.Y);
                }
            }
            depths.Sort();
        }

        /*
         * Render game elements in order of y-axis position.
         */
        public void Render(MainGame game, GameTime gameTime, GraphicsDevice GraphicsDevice, GraphicsDeviceManager graphics)
        {
            GraphicsDevice.Clear(backgroundColour); // Clear the graphics buffer and set the window background colour

            switch (currentMapEnum)
            {
                case CurrentMap.Intro:

                    render.StartDrawingSpriteBatch(camera.GetCamera());
                    text.GetText(GetTime(gameTime), render.TextSize(text.CurrentSentence), camera.PlayerPosition);
                    render.DrawText(text.TextPosition, text.CurrentText, text.TextOpacity);
                    render.FinishDrawingSpriteBatch();

                    if (freezeGame)
                    {
                        currentMenuItem = render.DrawStartMenu(
                            gameTime,
                            camera.PlayerPosition,
                            camera.GetCamera(),
                            camera.NewWidth,
                            camera.NewHeight,
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

                case CurrentMap.MainMenu: // If current screen is MainMenu
                    currentMenuItem = render.DrawMainMenu(
                        gameTime,
                        camera.PlayerPosition,
                        camera.GetCamera(),
                        camera.NewWidth,
                        camera.NewHeight,
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
                        render.StartDrawingMapSpriteBatch(camera.GetCamera());
                        render.DrawMap(map.GetFloorAtlas(), map, tileName.Key, tileName.Value);
                        render.FinishDrawingSpriteBatch();
                    }
                    if (hasFloorDecor)
                    {
                        foreach (var tileName in mapFloorDecor)
                        {
                            render.StartDrawingMapSpriteBatch(camera.GetCamera());
                            render.DrawMap(map.GetFloorDecorAtlas(), map, tileName.Key, tileName.Value);
                            render.FinishDrawingSpriteBatch();
                        }
                    }

                    RefreshDepths();

                    foreach (var depth in depths)
                    {
                        render.StartDrawingMapSpriteBatch(camera.GetCamera());
                        foreach (var tileName in mapLowerWalls)
                        {
                            if (tileName.Value.Y == depth)
                            {
                                render.DrawMap(map.GetWallAtlas(), map, tileName.Key, tileName.Value);
                            }
                        }
                        render.FinishDrawingSpriteBatch();
                        render.StartDrawingSpriteBatch(camera.GetCamera());
                        foreach (var scenery in Scenery.Values)
                        {
                            if (scenery.GetPosition().Y == depth)
                            {
                                render.StartDrawingSkeleton(GraphicsDevice, camera);
                                render.DrawScenerySkeleton(scenery);
                                render.FinishDrawingSkeleton();

                                render.DrawScenerySpriteBatch(scenery);
                            }
                        }
                        render.FinishDrawingSpriteBatch();
                        render.StartDrawingSpriteBatch(camera.GetCamera());
                        foreach (var entity in Entities.Values)
                        {
                            if (entity.GetPosition().Y == depth)
                            {
                                render.StartDrawingSkeleton(GraphicsDevice, camera);
                                render.DrawEntitySkeleton(entity);
                                render.FinishDrawingSkeleton();

                                render.DrawEntitySpriteBatch(entity);
                            }
                        }
                        render.FinishDrawingSpriteBatch();
                    }

                    if (hasUpperWalls)
                    {
                        foreach (var tileName in mapUpperWalls)
                        {
                            render.StartDrawingMapSpriteBatch(camera.GetCamera());
                            render.DrawMap(map.GetWallAtlas(), map, tileName.Key, tileName.Value);
                            render.FinishDrawingSpriteBatch();
                        }
                    }

                    if (freezeGame)
                    {
                        currentMenuItem = render.DrawStartMenu(
                            gameTime,
                            camera.PlayerPosition,
                            camera.GetCamera(),
                            camera.NewWidth,
                            camera.NewHeight,
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
                if (input == "A" || input == "Enter")
                {
                    /*if (intro.NextLine)
                    {
                        intro.StartNewLine();
                    }*/
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