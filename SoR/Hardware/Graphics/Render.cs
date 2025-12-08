using SoR.Logic.GameMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SoR.Logic.Character;
using Spine;
using System.Collections.Generic;
using SoR.Logic.GameMap.TiledScenery;

namespace SoR.Hardware.Graphics
{
    /*
     * Draw graphics to the screen, collect the impassable sections of the map, and convert map arrays into atlas positions
     * for drawing tiles.
     */
    internal partial class Render
    {
        private SpriteBatch spriteBatch;
        private SkeletonRenderer skeletonRenderer;
        private SpriteFont font;
        public List<Rectangle> ImpassableTiles { get; private set; }
        public Texture2D Curtain { get; set; }
        public Texture2D Str { get; set; }
        public Texture2D Con { get; set; }
        public Texture2D Agi { get; set; }
        public Texture2D Int { get; set; }

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
            Str = game.Content.Load<Texture2D>(Globals.GetResourcePath("Content\\SoR Resources\\Interface\\UI\\str"));
            Con = game.Content.Load<Texture2D>(Globals.GetResourcePath("Content\\SoR Resources\\Interface\\UI\\conv"));
            Agi = game.Content.Load<Texture2D>(Globals.GetResourcePath("Content\\SoR Resources\\Interface\\UI\\agi"));
            Int = game.Content.Load<Texture2D>(Globals.GetResourcePath("Content\\SoR Resources\\Interface\\UI\\int"));

            font = game.Content.Load<SpriteFont>("Fonts/File");

            ImpassableTiles = [];
        }

        /*
         * Draw the tiled backdrop.
         */
        public void DrawBackdrop(Backdrop backdrop, OrthographicCamera camera)
        {
            StartDrawingSpriteBatch(camera);

            if (backdrop.Position.X < backdrop.ScreenWidth)
            {
                spriteBatch.Draw(
                    backdrop.Tile,
                    backdrop.Position,
                    null,
                    Color.White,
                    0,
                    backdrop.Origin,
                    1,
                    SpriteEffects.None,
                    0f);
            }

            spriteBatch.Draw(
                backdrop.Tile,
                backdrop.Position - backdrop.TileSize,
                null,
                Color.White,
                0,
                backdrop.Origin,
                1,
                SpriteEffects.None,
                0f);

            FinishDrawingSpriteBatch();
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
         * Draw projectile skeletons.
         */
        public void DrawProjectileSkeleton(Projectile projectile)
        {
            // Draw skeletons
            skeletonRenderer.Draw(projectile.GetSkeleton());
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
         * Finish drawing SpriteBatch.
         */
        public void FinishDrawingSpriteBatch()
        {
            spriteBatch.End();
        }
    }
}