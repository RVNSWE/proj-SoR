using SoR.Logic.GameMap.TiledScenery;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;

namespace SoR.Hardware.Graphics
{
    /*
     * Draw graphics to the screen, collect the impassable sections of the map, and convert map arrays into atlas positions
     * for drawing tiles.
     */
    internal partial class Render
    {
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
