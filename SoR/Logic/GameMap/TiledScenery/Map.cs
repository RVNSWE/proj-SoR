using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using System;

namespace SoR.Logic.GameMap.TiledScenery
{
    /*
     * Create a map using a specified tileset and layout.
     */
    public class Map
    {
        private Texture2DAtlas floorAtlas;
        private Texture2DAtlas floorDecorAtlas;
        private Texture2DAtlas wallAtlas;
        private Texture2D floorTexture;
        private Texture2D floorDecorTexture;
        private Texture2D wallTexture;
        private int mapNumber;
        public string FloorSpriteSheet { get; set; }
        public string FloorDecorSpriteSheet { get; set; }
        public string WallSpriteSheet { get; set; }
        public int[,] Floor { get; set; }
        public int[,] FloorDecor { get; set; }
        public int[,] LowerWalls { get; set; }
        public int[,] UpperWalls { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public Map(int mapNumber)
        {
            this.mapNumber = mapNumber;
            GetMapLayout();
        }

        /*
         * Load map content.
         */
        public void LoadMap(ContentManager Content, string floorTileset, string wallTileset, string floorDecorTileset = null)
        {
            floorTexture = Content.Load<Texture2D>(floorTileset);
            wallTexture = Content.Load<Texture2D>(wallTileset);

            if (floorDecorTileset != null)
            {
                floorDecorTexture = Content.Load<Texture2D>(floorDecorTileset);
                floorDecorAtlas = Texture2DAtlas.Create("background decor", floorDecorTexture, TileWidth, TileHeight);
            }

            floorAtlas = Texture2DAtlas.Create("background", floorTexture, TileWidth, TileHeight);
            wallAtlas = Texture2DAtlas.Create("foreground", wallTexture, TileWidth, TileHeight);
        }

        /*
         * Create and return the temple wall layout.
         */
        public void GetMapLayout()
        {
            switch (mapNumber)
            {
                case 0:
                    LowerWalls = new int[,]
                    {
                        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1  },
                        { -1,  0,  2,  2,  2,  3,  2,  2,  2,  9  },
                        {  5,  1, -1, -1, -1, -1, -1, -1, -1,  23 },
                        {  5, -1, -1, -1, -1, -1, -1, -1, -1,  22 },
                        {  5, -1, -1, -1, -1, -1, -1, -1, -1,  9  },
                        {  10,-1, -1,  14, 16, 16, 15,-1, -1,  21 },
                        {  17, 19, 20, 18,-1, -1,  17, 19, 20, 18 }
                    };

                    UpperWalls = new int[,]
                    {
                        { -1,  6,  7,  7,  7,  7,  7,  7,  7,  8  },
                        {  4,  0, -1, -1, -1, -1, -1, -1, -1, -1  },
                        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1  },
                        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1  },
                        { -1, -1, -1,  12, 11, 11, 13,-1, -1, -1  },
                        { -1,  11, 11,-1, -1, -1, -1,  11, 11,-1  },
                        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1  }
                    };

                    Floor = new int[,]
                    {
                        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                        { -1, -1, -1, -1, -1,  0, -1, -1, -1, -1 },
                        { -1,  0,  2,  3,  0,  7,  0,  6,  5, -1 },
                        { -1,  0,  0,  0,  0,  0,  0,  0,  1, -1 },
                        { -1,  0,  0,  0,  0,  0,  0,  0,  0, -1 },
                        { -1,  4,  0, -1, -1, -1, -1,  0,  0, -1 },
                        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }
                    };

                    TileWidth = 64;
                    TileHeight = 64;

                    FloorSpriteSheet = "SoR Resources/Locations/TiledScenery/Temple/floorSpritesheet";
                    WallSpriteSheet = "SoR Resources/Locations/TiledScenery/Temple/wallSpritesheet";

                    break;

                case 1:
                    LowerWalls = new int[,]
                    {
                        { -1, -1, -1, -1, -1, -1, -1,  0,  1,  2  },
                        { -1, -1, -1, -1, -1, -1, -1,  3,  4,  5  },
                        { -1, -1, -1, -1,  0,  1,  2,  6,  7,  8  },
                        { -1, -1, -1, -1,  3,  4,  5, -1, -1, -1  },
                        { -1, -1, -1, -1,  6,  7,  8, -1, -1, -1  },
                        {  0,  1,  2, -1, -1, -1, -1, -1, -1, -1  },
                        {  3,  4,  5, -1, -1, -1, -1, -1, -1, -1  },
                        {  6,  7,  8, -1, -1, -1, -1,  0,  1,  2  },
                        { -1, -1, -1, -1, -1, -1, -1,  3,  4,  5  },
                        { -1, -1, -1, -1, -1, -1, -1,  6,  7,  8  },
                        { -1,  0,  1,  2, -1, -1, -1, -1, -1, -1  },
                        { -1,  3,  4,  5, -1, -1, -1, -1, -1, -1  },
                        { -1,  6,  7,  8, -1, -1, -1, -1, -1, -1  }
                    };

                    Floor = new int[,]
                    {
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) },
                        { new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15), new Random().Next(15) }
                    };

                    FloorDecor = new int[,]
                    {
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) },
                        { new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15), new Random().Next(-10, 15) }
                    };

                    TileWidth = 64;
                    TileHeight = 64;

                    FloorSpriteSheet = "SoR Resources/Locations/TiledScenery/Grass/spritesheet";
                    FloorDecorSpriteSheet = "SoR Resources/Locations/TiledScenery/Flowers/spritesheet";
                    WallSpriteSheet = "SoR Resources/Locations/Interactables/House/House";

                    break;

                case 2:
                    LowerWalls = new int[,]
                    {
                        {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1,  0, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0, -1, -1, -1, -1, -1, -1, -1, -1,  0  },
                        {  0,  0,  0,  0,  0,  0,  0,  0,  0,  0  }
                    };

                    TileWidth = 100;
                    TileHeight = 33;

                    FloorSpriteSheet = "SoR Resources/Locations/TiledScenery/Grass/spritesheet";
                    WallSpriteSheet = "SoR Resources/Locations/TiledScenery/Walls/1";

                    break;
            }
        }

        /*
         * Get the floor atlas.
         */
        public Texture2DAtlas GetFloorAtlas()
        {
            return floorAtlas;
        }

        /*
         * Get the floor decor atlas.
         */
        public Texture2DAtlas GetFloorDecorAtlas()
        {
            return floorDecorAtlas;
        }

        /*
         * Get the wall atlas.
         */
        public Texture2DAtlas GetWallAtlas()
        {
            return wallAtlas;
        }
    }
}