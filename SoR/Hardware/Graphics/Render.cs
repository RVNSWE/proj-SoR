using SoR.Logic.GameMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SoR.Logic.Character;
using Spine;
using System.Collections.Generic;

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
            font = game.Content.Load<SpriteFont>("Fonts/File");

            ImpassableTiles = [];
        }

        /*
         * Draw the curtain.
         */
        public void DrawCurtain(Vector2 position, OrthographicCamera camera, int width, int height, float fadeAlpha = 1f)
        {
            Vector2 backgroundPosition = new Vector2(position.X, position.Y);
            Vector2 scale = new Vector2(width, height);
            Vector2 adjustedPosition = new Vector2(backgroundPosition.X - (width / 2), backgroundPosition.Y - (height / 2));

            StartDrawingSpriteBatch(camera);
            spriteBatch.Draw(
                Curtain,
                adjustedPosition,
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
         * Finish drawing SpriteBatch.
         */
        public void FinishDrawingSpriteBatch()
        {
            spriteBatch.End();
        }
    }
}