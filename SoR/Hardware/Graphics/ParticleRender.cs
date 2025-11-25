using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;

namespace SoR.Hardware.Graphics
{
    public class ParticleRender
    {
        private SpriteBatch spriteBatch;
        private ParticleEffect particleEffect;
        private Texture2D particleTexture;

        public ParticleRender(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
    }
}
