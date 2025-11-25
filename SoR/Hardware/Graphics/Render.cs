using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended;
using SoR.Logic.Character;
using SoR.Logic.GameMap;
using SoR.Logic.GameMap.TiledScenery;
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
        private ParticleEffect particleEffect;
        private SkeletonRenderer skeletonRenderer;
        private SpriteFont font;
        private Texture2D particleTexture;
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

            // Create a simple 1x1 white texture for our particles
            particleTexture = new Texture2D(GraphicsDevice, 1, 1);
            particleTexture.SetData(new[] { Color.White });

            CreateParticleEffect(GraphicsDevice);
        }

        private void CreateParticleEffect(GraphicsDevice GraphicsDevice)
        {
            Vector2 viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            // Create the main effect container
            particleEffect = new ParticleEffect("Fire")
            {
                Position = viewportCenter,

                // Automatically trigger particle emitters
                AutoTrigger = true,

                // Emit particles every 0.1 seconds
                AutoTriggerFrequency = 0.1f
            };

            // Create the emitter that actually makes the particles.
            // With a capacity of 2000
            ParticleEmitter emitter = new ParticleEmitter(2000)
            {
                Name = "Fire Emitter",

                // Each particle created by this emitter lives for 2 seconds
                LifeSpan = 2.0f,
                TextureRegion = new Texture2DRegion(particleTexture),

                // Use a spray profile - particles emit in a directional cone
                Profile = Profile.Spray(-Vector2.UnitY, 2.0f),

                // Set up how particles look when they're created
                Parameters = new MonoGame.Extended.Particles.Data.ParticleReleaseParameters
                {
                    // Release 10-20 particles each time
                    Quantity = new MonoGame.Extended.Particles.Data.ParticleInt32Parameter(10, 20),

                    // Random speed between 10-40
                    Speed = new MonoGame.Extended.Particles.Data.ParticleFloatParameter(10.0f, 40.0f),

                    // Red color  using HSL values (Hue=0°, Saturation = 100%, Lightness=60%)
                    Color = new MonoGame.Extended.Particles.Data.ParticleColorParameter(new Vector3(0.0f, 1.0f, 0.6f)),

                    // Make them 10x bigger
                    Scale = new MonoGame.Extended.Particles.Data.ParticleVector2Parameter(new Vector2(10f, 10f))
                }
            };

            // Add fire-like behavior
            emitter.Modifiers.Add(new LinearGravityModifier
            {
                // Point upward (negative Y)
                Direction = -Vector2.UnitY,

                // Make fire rise with this much force
                Strength = 100f
            });

            // Make particles fade out as they age
            emitter.Modifiers.Add(new AgeModifier
            {
                Interpolators =
        {
            new OpacityInterpolator
            {
                // Start fully visible
                StartValue = 1.0f,

                // Fade to transparent over lifetime
                EndValue = 0.0f
            }
        }
            });

            // Add the emitter to our effect
            particleEffect.Emitters.Add(emitter);
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