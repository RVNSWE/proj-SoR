using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System.Collections.Generic;
using Newtonsoft.Json;
using SoR.Hardware.Input;

namespace SoR.Logic.Character.Player
{
    /*
     * Stores information unique to Player.
     */
    internal partial class Player : Entity
    {
        protected GamePadInput gamePadInput;
        protected KeyboardInput keyboardInput;
        protected string lastAnimation;
        protected string movementAnimation;

        [JsonConstructor]
        public Player(GraphicsDevice GraphicsDevice, List<Rectangle> impassableArea)
        {
            // The possible animations to play as a string and the method to use for playing them as an int
            animations = new Dictionary<string, int>()
            {
                { "idleup", 1 },
                { "idledown", 1 },
                { "idleleft", 1 },
                { "idleright", 1 },
                { "runup", 3 },
                { "rundown", 3 },
                { "runleft", 3 },
                { "runright", 3 }
            };

            // Load texture atlas and attachment loader
            atlas = new Atlas(Globals.GetResourcePath("Content\\SoR Resources\\Entities\\Player\\MC2.atlas"), new XnaTextureLoader(GraphicsDevice));
            atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
            json = new SkeletonJson(atlasAttachmentLoader);
            json.Scale = 0.33f;

            // Initialise skeleton json
            skeletonData = json.ReadSkeletonData(Globals.GetResourcePath("Content\\SoR Resources\\Entities\\Player\\skeleton.json"));
            skeleton = new Skeleton(skeletonData);

            // Set the skin
            skeleton.SetSkin(skeletonData.FindSkin("down"));
            Skin = "down";

            // Setup animation
            animStateData = new AnimationStateData(skeleton.Data);
            animStateData.DefaultMix = 0.1f;
            animState = new AnimationState(animStateData);
            animState.Apply(skeleton);

            // Set the "fidle" animation on track 1 and leave it looping forever
            trackEntry = animState.SetAnimation(0, "idledown", true);

            // Create hitbox
            slot = skeleton.FindSlot("hitbox");
            hitboxAttachment = skeleton.GetAttachment("hitbox", "hitbox");
            slot.Attachment = hitboxAttachment;
            skeleton.SetAttachment("hitbox", "hitbox");

            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            gamePadInput = new GamePadInput();
            keyboardInput = new KeyboardInput();

            idle = true; // Player is currently idle
            lastAnimation = ""; // Get the last key pressed

            Traversable = true; // Whether the entity is on walkable terrain

            CountDistance = 0; // Count how far to automatically move the entity
            direction = new Vector2(0, 0); // The direction of movement
            BeenPushed = false;
            sinceFreeze = 0; // Time since entity movement was frozen
            isFacing = "idledown";

            Player = true;

            Speed = 120; // Set the entity's travel speed
            HitPoints = 100; // Set the starting number of hitpoints

            ImpassableArea = impassableArea;
        }

        /*
         * Placeholder function for handling battles.
         */
        public void Battle(Entity entity)
        {
            /*
                    If (entity.CollidesWith(player))
                    {
                        player.Battle(entity);
                    }
             */
        }

        /*
         * If something changes to trigger a new animation, apply the animation.
         * If the animation is already applied, do nothing.
         */
        public override void ChangeAnimation(string eventTrigger)
        {
            string reaction; // Null if there will be no animation change

            if (prevTrigger != eventTrigger)
            {
                foreach (string animation in animations.Keys)
                {
                    if (eventTrigger == animation)
                    {
                        prevTrigger = animOne = reaction = animation;
                        animTwo = isFacing;

                        React(reaction, animations[animation]);
                    }
                }
            }
        }

        /*
         * Define what happens on collision with an entity.
         */
        public override void EntityCollision(Entity entity, GameTime gameTime)
        {
            entity.TakeDamage(1);
            entity.ChangeAnimation("hit");
            RepelledFromEntity(4, entity);
        }

        /*
         * Change the player x-axis direction according to keyboard input.
         */
        public void MovementDirectionX(int changeDirection)
        {
            if (changeDirection != 0)
            {
                direction.X = changeDirection;
            }
        }

        /*
         * Change the player y-axis direction according to keyboard input.
         */
        public void MovementDirectionY(int changeDirection)
        {
            if (changeDirection != 0)
            {
                direction.Y = changeDirection;
            }
        }

        /*
         * Update entity position.
         */
        public override void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            FrozenTimer(gameTime);

            if (!Frozen)
            {
                keyboardInput.GetInput();
                gamePadInput.GetInput();

                CheckIdle();

                if (keyboardInput.CurrentInputDevice)
                {
                    ProcessXMovementInput(keyboardInput.X);
                    ProcessYMovementInput(keyboardInput.Y);
                }
                else
                {
                    ProcessXMovementInput(gamePadInput.X);
                    ProcessYMovementInput(gamePadInput.Y);
                }

                BeMoved(gameTime);

                AdjustPosition(gameTime, ImpassableArea);

                lastAnimation = movementAnimation;
            }
        }

        /*
         * Update the skeleton position, skin and animation state.
         */
        public override void UpdateAnimations(GameTime gameTime)
        {
            ChangeAnimation(movementAnimation);

            base.UpdateAnimations(gameTime);
        }
    }
}