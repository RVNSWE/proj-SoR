using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoR.Logic.GameMap.TiledScenery
{
    /*
     * Create a tiled background.
     */
    public class Backdrop
    {
        public Vector2 TileSize;
        public Vector2 Origin;
        public int ScreenWidth;
        public int ScreenHeight;
        public Vector2 Position;
        public Texture2D Tile;
        public Rectangle TargetRectangle;

        /*
         * Load the tiled background.
         */
        public void Load(MainGame game, GraphicsDevice GraphicsDevice)
        {
            Tile = game.Content.Load<Texture2D>("SoR Resources/Locations/TiledScenery/Backdrops/nightsky");

            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            Origin = new Vector2(Tile.Width / 2, 0);
            Position = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            TileSize = new Vector2(0, Tile.Height);
        }

        /*
         * Update the background position.
         */
        public void Update(Vector2 position)
        {
            Position.X = position.X * 2;
            Position.Y = position.Y * 2;
        }
    }
}