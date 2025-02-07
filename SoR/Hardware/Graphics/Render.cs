using SoR.Logic.GameMap;
using SoR.Logic.GameMap.TiledScenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using SoR.Logic.Character;
using Spine;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoR.Hardware.Graphics
{
    /*
     * Draw graphics to the screen, collect the impassable sections of the map, and convert map arrays into atlas positions
     * for drawing tiles.
     */
    internal class Render
    {
        private SpriteBatch spriteBatch;
        private SkeletonRenderer skeletonRenderer;
        public List<Rectangle> ImpassableTiles { get; private set; }
        public Texture2D Curtain { get; set; }

        /*
         * Initialise the SpriteBatch, SkeletonRenderer and ImpassableTiles collection.
         */
        public Render(MainGame game, GraphicsDevice GraphicsDevice)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            skeletonRenderer = new SkeletonRenderer(GraphicsDevice)
            {
                PremultipliedAlpha = true
            };

            Curtain = game.Content.Load<Texture2D>(Globals.GetResourcePath("Content\\SoR Resources\\Screens\\Screen Transitions\\curtain"));

            ImpassableTiles = [];
        }

        /*
         * Draw the curtain.
         */
        public void DrawCurtain(float positionX, float positionY, OrthographicCamera camera, int width, int height, Texture2D curtain, float fadeAlpha = 1f)
        {
            Vector2 backgroundPosition = new Vector2(positionX, positionY);
            Vector2 scale = new Vector2(width, height);
            Vector2 position = new Vector2(backgroundPosition.X - (width / 2), backgroundPosition.Y - (height / 2));

            StartDrawingSpriteBatch(camera);
            spriteBatch.Draw(
                Curtain,
                position,
                null,
                Color.White * fadeAlpha,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
            FinishDrawingSpriteBatch();
        }
        
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
            Texture2D curtain,
            SpriteFont font,
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

            DrawCurtain(positionX, positionY, camera, width, height, curtain);

            StartDrawingSpriteBatch(camera);
            MenuText(title, titlePosition, font, Color.GhostWhite, 2.5f);
            MenuText(newGame, newGamePosition, font, Color.Gray, 1);
            MenuText(continueGame, continueGamePosition, font, Color.Gray, 1);
            MenuText(loadGame, loadGamePosition, font, Color.Gray, 1);
            MenuText(settings, gameSettingsPosition, font, Color.Gray, 1);
            MenuText(exitGame, toDesktopPosition, font, Color.Gray, 1);
            FinishDrawingSpriteBatch();

            switch (select)
            {
                case 0:
                    StartDrawingSpriteBatch(camera);
                    MenuText(newGame, newGamePosition, font, Color.GhostWhite, 1);
                    FinishDrawingSpriteBatch();
                    currentMenuItem = newGame;
                    break;
                case 1:
                    if (File.Exists(saveFile))
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(continueGame, continueGamePosition, font, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = continueGame;
                    }
                    else
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(continueGame, continueGamePosition, font, Color.LightCoral, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = "none";
                    }
                    break;
                case 2:
                    if (File.Exists(saveFile))
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(loadGame, loadGamePosition, font, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = loadGame;
                    }
                    else
                    {
                        StartDrawingSpriteBatch(camera);
                        MenuText(loadGame, loadGamePosition, font, Color.LightCoral, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = "none";
                    }
                    break;
                case 3:
                    StartDrawingSpriteBatch(camera);
                    MenuText(settings, gameSettingsPosition, font, Color.GhostWhite, 1);
                    FinishDrawingSpriteBatch();
                    currentMenuItem = settings;
                    break;
                case 4:
                    StartDrawingSpriteBatch(camera);
                    MenuText(exitGame, toDesktopPosition, font, Color.GhostWhite, 1);
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
            Texture2D curtain,
            SpriteFont font,
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

            DrawCurtain(positionX, positionY, camera, width, height, curtain, 0.75f);

            StartDrawingSpriteBatch(camera);
            MenuText(inventory, inventoryPosition, font, Color.Gray, 1);
            MenuText(settings, gameSettingsPosition, font, Color.Gray, 1);
            MenuText(loadGame, loadGamePosition, font, Color.Gray, 1);
            MenuText(exitGame, exitGamePosition, font, Color.Gray, 1);
            FinishDrawingSpriteBatch();

            if (!exitMenu)
            {
                switch (select)
                {
                    case 0:
                        StartDrawingSpriteBatch(camera);
                        MenuText(inventory, inventoryPosition, font, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = inventory;
                        break;
                    case 1:
                        StartDrawingSpriteBatch(camera);
                        MenuText(settings, gameSettingsPosition, font, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = settings;
                        break;
                    case 2:
                        if (File.Exists(saveFile))
                        {
                            StartDrawingSpriteBatch(camera);
                            MenuText(loadGame, loadGamePosition, font, Color.GhostWhite, 1);
                            FinishDrawingSpriteBatch();
                            currentMenuItem = loadGame;
                        }
                        else
                        {
                            StartDrawingSpriteBatch(camera);
                            MenuText(loadGame, loadGamePosition, font, Color.LightCoral, 1);
                            FinishDrawingSpriteBatch();
                            currentMenuItem = "none";
                        }
                        break;
                    case 3:
                        StartDrawingSpriteBatch(camera);
                        MenuText(exitGame, exitGamePosition, font, Color.GhostWhite, 1);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = exitGame;
                        break;
                }
            }
            else
            {
                DrawCurtain(positionX, positionY, camera, width, height, curtain, 0.15f);

                StartDrawingSpriteBatch(camera);
                MenuText(toMain, toMainMenuPosition, font, Color.Gray, 1.25f);
                MenuText(toDesktop, toDesktopPosition, font, Color.Gray, 1.25f);
                FinishDrawingSpriteBatch();

                switch (select)
                {
                    case 0:
                        StartDrawingSpriteBatch(camera);
                        MenuText(toMain, toMainMenuPosition, font, Color.GhostWhite, 1.25f);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = toMain;
                        break;
                    case 1:
                        StartDrawingSpriteBatch(camera);
                        MenuText(toDesktop, toDesktopPosition, font, Color.GhostWhite, 1.25f);
                        FinishDrawingSpriteBatch();
                        currentMenuItem = toDesktop;
                        break;
                }
            }

            return currentMenuItem;
        }

        /*
         * Start drawing skeletons.
         */
        public void StartDrawingSkeleton(GraphicsDevice GraphicsDevice, Camera camera)
        {
            ((BasicEffect)skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(
                    0,
                        GraphicsDevice.Viewport.Width,
                        GraphicsDevice.Viewport.Height,
                        0, 1, 0);
            ((BasicEffect)skeletonRenderer.Effect).View = camera.GetCamera().GetViewMatrix();

            skeletonRenderer.Begin();
        }

        /*
         * Draw entity skeletons.
         */
        public void DrawEntitySkeleton(Entity entity)
        {
            // Draw skeletons
            skeletonRenderer.Draw(entity.GetSkeleton());
        }

        /*
         * Draw scenery skeletons.
         */
        public void DrawScenerySkeleton(Scenery scenery)
        {
            // Draw skeletons
            skeletonRenderer.Draw(scenery.GetSkeleton());
        }

        /*
         * Finish drawing Skeleton.
         */
        public void FinishDrawingSkeleton()
        {
            skeletonRenderer.End();
        }

        /*
         * Start drawing SpriteBatch.
         */
        public void StartDrawingSpriteBatch(OrthographicCamera camera)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
        }

        /*
         * Start drawing map SpriteBatch.
         * 
         * SamplerState.PointClamp snaps to pixels - fixes misaligned map tiles.
         */
        public void StartDrawingMapSpriteBatch(OrthographicCamera camera)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            //samplerState: SamplerState.PointClamp,
            //blendState: BlendState.AlphaBlend);
        }

        /*
         * Draw SpriteBatch for entities.
         */
        public void DrawEntitySpriteBatch(Entity entity, SpriteFont font)
        {
            // Entity text
            spriteBatch.DrawString(
                font,
                "HP: " + entity.GetHitPoints()
                + "\nX: " + entity.GetPosition().X + " Y: " + entity.GetPosition().Y,
                new Vector2(entity.GetPosition().X - 30, entity.GetPosition().Y + 30),
                Color.BlueViolet);
        }

        /*
         * Draw SpriteBatch for scenery.
         */
        public void DrawScenerySpriteBatch(Scenery scenery, SpriteFont font)
        {
            // Scenery text
            spriteBatch.DrawString(
                font,
                "X: " + scenery.GetPosition().X + " Y: " + scenery.GetPosition().Y,
                new Vector2(scenery.GetPosition().X - 80, scenery.GetPosition().Y + 100),
                Color.BlueViolet);
        }

        /*
         * Finish drawing SpriteBatch.
         */
        public void FinishDrawingSpriteBatch()
        {
            spriteBatch.End();
        }

        /*
         * Draw the text for the main menu.
         */
        public void MenuText(string menuItem, Vector2 position, SpriteFont font, Color colour, float scale)
        {
            // Entity text
            spriteBatch.DrawString(
            font,
                menuItem,
                new Vector2(position.X, position.Y),
                colour,
                0,
                new Vector2(0, 0),
                scale,
                SpriteEffects.None,
                0);
        }

        /*
         * Pair the atlas position of each tile with its world position.
         */
        public Dictionary<string, Vector2> CreateMap(Map map, int[,] tileLocations, bool impassable = false)
        {
            Dictionary<string, Vector2> sortByYAxis = [];
            Vector2 position = new(0, 0);
            int tileID = 1000;
            int row = 0;
            int column = 1;

            for (int x = 0; x < tileLocations.GetLength(row); x++) // For each row in the 2D array
            {
                for (int y = 0; y < tileLocations.GetLength(column); y++) // For each column
                {
                    int tile = tileLocations[x, y]; // Get the value of the current tile

                    if (tile > -1) // If it's a renderable tile
                    {
                        string tileName = string.Concat(tileID + tile.ToString()); // Give it a unique ID
                        sortByYAxis.Add(tileName, position); // And add it to the collection
                        tileID++; // Iterate the ID by 1

                        if (impassable) // If it's an impassable tile
                        {
                            Rectangle tileArea = new Rectangle((int)position.X, (int)position.Y, map.Width, map.Height); // Get the area of the map it occupies
                            ImpassableTiles.Add(tileArea); // And add it to the collection of impassable tile spaces
                        }
                    }

                    position.X += map.Width; // Step right by one tile space
                }
                position.X = 0;
                position.Y += map.Height; // Reset the x-axis and step down by one tile space
            }

            return sortByYAxis;
        }

        /*
         * Group impassable tile spaces into a collection of rows containing the impassable map areas.
         */
        public void ImpassableMapArea()
        {
            Rectangle block = ImpassableTiles[0]; // Take the first impassable tile space in the collection as the first block to compare
            List<Rectangle> impassableArea = []; // Ready a temporary empty collection to store the grouped impassable map areas

            foreach (Rectangle area in ImpassableTiles) // For each impassable tile space in the collection created on map generation
            {
                if (block.Bottom != area.Bottom) // If the block being compared against is not in the same row as, or is taller than, this tile space
                {
                    AddBlock(block, impassableArea); // Add it to the new collection
                    block = area; // Take the current impassable tile space as the next block to compare
                }
                if (block.Bottom == area.Bottom && block.Y == area.Y) // If this block is in the same row as this space and is the same height
                {
                    if (block.Right == area.Left) // If its x-axis end point is the same as this area's start point
                    {
                        block.Width += area.Width; // Combine their widths so the block encompasses both spaces
                    }
                    else if (block != area) // Otherwise, provided they do not represent the same tile space
                    {
                        AddBlock(block, impassableArea); // Add this block to the collection of impassable map spaces
                        block = area; // Take the current impassable tile space as the next block to compare
                    }
                }
            }
            AddBlock(block, impassableArea); // Add the final block to the collection of impassable map areas once there are no more tile spaces to compare against

            ImpassableTiles = impassableArea; // Update the original collection with the new collection
        }

        /*
         * Add the current block to the list of walkable areas.
         */
        public void AddBlock(Rectangle block, List<Rectangle> impassableArea)
        {
            block.Y -= block.Height;
            impassableArea.Add(block);
        }

        /*
         * Draw the map to the screen.
         */
        public void DrawMap(Texture2DAtlas atlas, Map map, string tileName, Vector2 position)
        {
            Vector2 blockPosition = position;

            string tile = tileName.Remove(0, 4); // Remove the unique ID to get the atlas position
            int tileNumber = Convert.ToInt32(tile);

            // Offset drawing position by tile height to draw in front of any components using a different positioning reference
            blockPosition.Y -= map.Height * 1.25f;

            blockPosition.X = (int)blockPosition.X;
            blockPosition.Y = (int)blockPosition.Y;
            spriteBatch.Draw(atlas[tileNumber], blockPosition, Color.White); // Draw the tile to the screen
        }
    }
}