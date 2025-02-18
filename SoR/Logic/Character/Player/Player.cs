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
                { "runright", 3 },
                { "sitdown", 2 },
                { "sittingdown", 1 },
                { "standdown", 2 },
                { "situp", 2 },
                { "sittingup", 1 },
                { "standup", 2 },
                { "sitleft", 2 },
                { "sittingleft", 1 },
                { "standleft", 2 },
                { "sitright", 2 },
                { "sittingright", 1 },
                { "standright", 2 }
            };

            // Load texture atlas and attachment loader
            atlas = new Atlas(Globals.GetResourcePath("Content\\SoR Resources\\Entities\\Player\\MC3.atlas"), new XnaTextureLoader(GraphicsDevice));
            atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
            json = new SkeletonJson(atlasAttachmentLoader);
            json.Scale = 0.5f;

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

            Pausing = false;
            Colliding = false;
            idle = true;
            lastAnimation = "";
            prevTrigger = "";
            animOne = "";
            animTwo = "";
            isFacing = "idledown";
            movementAnimation = "idledown";

            Traversable = true;

            CountDistance = 0;
            direction = new Vector2(0, 0);
            prevDirection = direction;
            BeenPushed = false;
            collisionSeconds = 0;
            frozenSeconds = 1;
            pauseSeconds = 0;
            newSpeed = 0;

            Player = true;
            Name = "Mercura";

            Speed = 120;
            HitPoints = 100;

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

            if (prevTrigger != eventTrigger && animations.TryGetValue(eventTrigger, out int animType))
            {
                prevTrigger = animOne = reaction = eventTrigger;
                animTwo = isFacing;
                React(reaction, animType);
            }
        }

        /*
         * Define what happens on collision with an entity.
         */
        public override void EntityCollision(Entity entity, GameTime gameTime)
        {
            if (!Colliding)
            {
                TakeDamage(1);
                collisionSeconds = 1;
                Colliding = true;
            }

            RepelledFromEntity(5, entity);
        }

        /*
         * Change the player x-axis direction according to keyboard input.
         */
        public void MovementDirectionX(int adjustDirection)
        {
            if (adjustDirection != 0)
            {
                direction.X = adjustDirection;
            }
        }

        /*
         * Change the player y-axis direction according to keyboard input.
         */
        public void MovementDirectionY(int adjustDirection)
        {
            if (adjustDirection != 0)
            {
                direction.Y = adjustDirection;
            }
        }

        /*
         * Update entity position.
         */
        public override void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            CheckIfFrozen(gameTime);
            WaitForCollisionSeconds(gameTime);

            if (!Frozen)
            {
                BeMoved(gameTime);

                keyboardInput.GetInput();
                gamePadInput.GetInput();

                CheckIdle();
                CheckSitting();

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

                CalculateSpeed(gameTime);
                AdjustXPosition(ImpassableArea);
                AdjustYPosition(ImpassableArea);
            }
        }
    }
}