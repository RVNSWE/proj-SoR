using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoR.Hardware.Input;
using SoR.Logic.Character.Projectiles;
using Spine;
using System.Collections.Generic;
using static System.Windows.Forms.AxHost;

namespace SoR.Logic.Character.Player
{
    /*
     * Spine Runtimes License
     */
    /**************************************************************************************************************************
     * Copyright (c) 2013-2024, Esoteric Software LLC
     * 
     * Integration of the Spine Runtimes into software or otherwise creating derivative works of the Spine Runtimes is
     * permitted under the terms and conditions of Section 2 of the Spine Editor License Agreement:
     * http://esotericsoftware.com/spine-editor-license
     * 
     * Otherwise, it is permitted to integrate the Spine Runtimes into software or otherwise create derivative works of the
     * Spine Runtimes (collectively, "Products"), provided that each user of the Products must obtain their own Spine Editor
     * license and redistribution of the Products in any form must include this license and copyright notice.
     * 
     * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT
     * NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
     * EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
     * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
     * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
     * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SPINE RUNTIMES, EVEN IF ADVISED OF THE
     * POSSIBILITY OF SUCH DAMAGE.
     **************************************************************************************************************************/
    /*
     * Stores information unique to Player.
     */
    internal partial class Player : Entity
    {
        private GamePadInput gamePadInput;
        private KeyboardInput keyboardInput;

        [JsonConstructor]
        public Player(GraphicsDevice GraphicsDevice, List<Rectangle> impassableArea)
        {
            // The possible animations to play as a string and the method to use for playing them as an int
            animations = new Dictionary<string, int>()
            {
                { "D_idle", 1 },
                { "U_idle", 1 },
                { "L_idle", 1 },
                { "R_idle", 1 },
                { "D_run", 3 },
                { "U_run", 3 },
                { "L_run", 3 },
                { "R_run", 3 },
                { "D_sit", 2 },
                { "D_sitting", 1 },
                { "D_stand", 2 },
                { "U_up", 2 },
                { "U_sitting", 1 },
                { "U_stand", 2 },
                { "L_sit", 2 },
                { "L_sitting", 1 },
                { "L_stand", 2 },
                { "R_sit", 2 },
                { "R_sitting", 1 },
                { "R_stand", 2 }
            };

            Stats = new Dictionary<string, float>()
            {
                { "STR", 120f },
                { "CON", 0f },
                { "AGI", 20f },
                { "INT", 0f }
            };

            // Load texture atlas and attachment loader
            atlas = new Atlas(Globals.GetResourcePath("Content\\SoR Resources\\Entities\\Player\\MC4.atlas"), new XnaTextureLoader(GraphicsDevice));
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
            trackEntry = animState.SetAnimation(0, "D_idle", true);

            // Create hitbox
            slot = skeleton.FindSlot("hitbox");
            hitboxAttachment = skeleton.GetAttachment("hitbox", "hitbox");
            slot.Attachment = hitboxAttachment;
            skeleton.SetAttachment("hitbox", "hitbox");

            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            gamePadInput = new GamePadInput();
            keyboardInput = new KeyboardInput();

            GamePaused = false;
            Pausing = false;
            Colliding = false;
            Casting = false;
            idle = true;
            lastAnimation = "";
            prevTrigger = "";
            animOne = "";
            animTwo = "";
            isFacing = "D_idle";
            movementAnimation = "D_idle";

            Traversable = true;

            CountDistance = 0;
            direction = new Vector2(0, 0);
            prevDirection = direction;
            BeenPushed = false;
            collisionSeconds = 0;
            frozenSeconds = 1;
            pauseSeconds = 0;

            Speed = 120;
            newSpeed = 0;
            HitPoints = 100;

            Player = true;
            Name = "Capricia";

            ImpassableArea = impassableArea;

            Projectiles = [];
        }

        /*
         * Choose projectile to create.
         */
        public override void CreateProjectile(string projectileType, GraphicsDevice GraphicsDevice, float positionX, float positionY)
        {
            switch (projectileType)
            {
                case "fireball":
                    if (!Projectiles.ContainsKey("fireball"))
                    {
                        Projectiles.Add("fireball", new Fireball(GraphicsDevice, ImpassableArea) { Name = "fireball" });
                        if (Projectiles.TryGetValue("fireball", out Projectile fireball))
                        {
                            fireball.SetPosition(positionX, positionY);
                            fireball.Appear();
                            Casting = true;
                        }
                    }
                    break;
            }
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
                collisionSeconds = 1;
                Colliding = true;
            }

            RepelledDistanceFrom(0.1f, entity.GetPosition().X, entity.GetPosition().Y);
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
         * Update entity Position.
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

                if (Projectiles.TryGetValue("fireball", out Projectile fireball))
                {
                    if (Casting)
                    {
                        UpdateProjectile(gameTime, fireball);
                    }
                    else
                    {
                        LaunchProjectile(fireball);
                    }

                    if (fireball.Colliding)
                    {
                        fireball.CountDistance = 0;
                        fireball.Vanish();
                    }
                    if (fireball.LifeTime <= 0.3f)
                    {
                        float deltaTime = GameLogic.GetTime(gameTime);
                        fireball.LifeTime -= deltaTime;
                    }
                    if (fireball.LifeTime <= 0)
                    {
                        Projectiles.Remove(fireball.Name);
                        Casting = false;
                    }

                    fireball.UpdatePosition(gameTime, graphics);
                }
            }
        }

        /*
         * Projectile follows the Player hand bone, plays vanish animation shortly before
         * disappearing, and is removed from Projectiles when intn runs out.
         */
        public void UpdateProjectile(GameTime gameTime, Projectile projectile)
        {
            Bone handBone = skeleton.FindBone(CheckHand());
            projectile.SetPosition(handBone.WorldX, handBone.WorldY);

            if (CheckHand() == "HB")
            {
                projectile.Behind = true;
            }
            else
            {
                projectile.Behind = false;
            }

            if (GetStatValue("INT") >= 0)
            {
                float deltaTime = GameLogic.GetTime(gameTime);
                float newValue = GetStatValue("INT") - deltaTime;
                UpdateStats(gameTime);
            }
            if (GetStatValue("INT") <= 0.3f)
            {
                projectile.Vanish();
            }
        }

        /*
         * Launch the projectile away from the Player.
         */
        public void LaunchProjectile(Projectile projectile)
        {
            if (!projectile.Cast)
            {
                Bone handBone = skeleton.FindBone(CheckHand());
                float x = handBone.WorldX;
                float y = handBone.WorldY;

                if (idle)
                {
                    switch (isFacing)
                    {
                        case "L_idle":
                            x += 5;
                            break;
                        case "R_idle":
                            x -= 5;
                            break;
                        case "U_idle":
                            y += 5;
                            break;
                        case "D_idle":
                            y -= 5;
                            break;
                    }
                }
                else
                {
                    float modifiedX = direction.X * 5;
                    float modifiedY = direction.Y * 5;
                    x = projectile.GetPosition().X - modifiedX;
                    y = projectile.GetPosition().Y - modifiedY;
                }

                projectile.LaunchDistanceFromXY(projectile.LifeTime, x, y);
                projectile.Cast = true;
            }
            if (projectile.CountDistance <= 0.3f)
            {
                projectile.Vanish();
            }
        }

        public override void UpdateStats(GameTime gameTime)
        {
            float deltaTime = GameLogic.GetTime(gameTime);
            float newValue;

            if (GamePaused)
            {
                return;
            }

            foreach (var stat in Stats)
            {
                switch (stat.Key)
                {
                    case "STR":
                        break;
                    case "CON":
                        break;
                    case "AGI":
                        break;
                    case "INT":
                        if (Casting && GetStatValue(stat.Key) > 0)
                        {
                            newValue = GetStatValue(stat.Key) - deltaTime;
                            UpdateStats(gameTime);
                        }
                        else if (GetStatValue(stat.Key) < maxStatValue)
                        {
                            newValue = GetStatValue(stat.Key) + deltaTime;
                            Stats.Remove(stat.Key);
                            Stats.Add(stat.Key, newValue);
                        }
                        break;
                }

                if (stat.Value < 0)
                {
                    Stats.Remove(stat.Key);
                    Stats.Add(stat.Key, 0);
                }
                if (stat.Value > maxStatValue)
                {
                    Stats.Remove(stat.Key);
                    Stats.Add(stat.Key, maxStatValue);
                }
            }
        }
    }
}