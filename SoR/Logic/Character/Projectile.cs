using Microsoft.Xna.Framework;
using SoR.Logic.GameMap;
using Spine;
using System.Collections.Generic;

namespace SoR.Logic.Character
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
     * Parent class for player and non-player characters.
     */
    public partial class Projectile
    {
        protected Dictionary<string, int> animations;
        protected Atlas atlas;
        protected AtlasAttachmentLoader atlasAttachmentLoader;
        protected SkeletonJson json;
        protected SkeletonData skeletonData;
        protected SkeletonRenderer skeletonRenderer;
        protected Skeleton skeleton;
        protected AnimationStateData animStateData;
        protected AnimationState animState;
        protected Attachment hitboxAttachment;
        protected SkeletonBounds hitbox;
        protected Slot slot;
        protected TrackEntry trackEntry;
        protected Vector2 position;
        protected string prevTrigger;
        protected string animOne;
        protected string animTwo;
        protected string defaultAnim;
        protected string lastAnimation;
        protected string movementAnimation;
        protected string isFacing;
        protected float newSpeed;
        protected string waitType;
        public List<Rectangle> ImpassableArea { get; protected set; } // Public, as this may vary
        public string Type { get; set; }
        public int Speed { get; set; }
        public string Skin { get; set; }
        public bool Colliding { get; set; }
        public bool Pausing { get; set; }
        public string Name { get; set; }

        /*
         * Update skin after loading game or changing screens.
         */
        public void UpdateSkin(string skin)
        {
            skeleton.SetSkin(skeletonData.FindSkin(skin));
            skeleton.SetSlotsToSetupPose();
            animState.Apply(skeleton);
        }

        /*
         * If something changes to trigger a new animation, apply the animation.
         * If the animation is already applied, do nothing.
         */
        public virtual void ChangeAnimation(string eventTrigger)
        {
            string reaction; // Null if there will be no animation change

            if (prevTrigger != eventTrigger && animations.TryGetValue(eventTrigger, out int animType))
            {
                animTwo = defaultAnim;
                prevTrigger = animOne = reaction = eventTrigger;
                React(reaction, animType);
            }
        }

        /*
         * Choose a method for playing the animation according to ChangeAnimation(eventTrigger)
         * animType.
         * 
         * 1 = rapidly transition to next animation
         * 2 = set new animation then queue the next
         * 3 = start next animation on the same frame the previous animation finished on
         */
        public void React(string reaction, int animType)
        {
            if (!string.IsNullOrEmpty(reaction))
            {
                switch (animType)
                {
                    case 1:
                        if (trackEntry != null)
                        {
                            animState.SetAnimation(0, animOne, true);
                        }
                        else
                        {
                            animState.AddAnimation(0, animOne, true, trackEntry.TrackComplete);
                        }
                        break;
                    case 2:
                        animState.SetAnimation(0, animOne, false);
                        animState.AddAnimation(0, animTwo, true, 0);
                        break;
                    case 3:
                        if (trackEntry != null) // If there's a queue then buttons are being mashed, so just clear it and set next.
                        {
                            animState.SetAnimation(0, animOne, true);
                        }
                        else // Otherwise, add next to start on frame current anim finished on.
                        {
                            trackEntry = animState.AddAnimation(0, animOne, true, trackEntry.TrackTime);
                        }
                        break;
                }
            }
        }

        /*
         * Check for collision with other entities.
         */
        public bool CollidesWith(Entity entity)
        {
            entity.UpdateHitbox(new SkeletonBounds());
            entity.GetHitbox().Update(entity.GetSkeleton(), true);

            hitbox = new SkeletonBounds();
            hitbox.Update(skeleton, true);

            if (hitbox.AabbIntersectsSkeleton(entity.GetHitbox()))
            {
                return true;
            }

            return false;
        }

        /*
         * Be automatically moved.
         */
        public void BeMoved(GameTime gameTime)
        {
            if (CountDistance > 0)
            {
                if (CountDistance == 1)
                {
                    direction = Vector2.Zero;
                    BeenPushed = true;
                }

                CalculateSpeed(gameTime);
                AdjustXPosition(ImpassableArea);
                AdjustYPosition(ImpassableArea);

                CountDistance--;
            }
        }

        /*
         * Wait for a collision.
         */
        public void WaitForCollisionSeconds(GameTime gameTime)
        {
            if (collisionSeconds > 0)
            {
                collisionSeconds = SecondsRemaining(gameTime, collisionSeconds);
            }
            else
            {
                Colliding = false;
            }
        }

        /*
         * Wait to move.
         */
        public void WaitForPauseSeconds(GameTime gameTime)
        {
            if (pauseSeconds > 0)
            {
                pauseSeconds = SecondsRemaining(gameTime, pauseSeconds);
            }
            else
            {
                Pausing = false;
            }
        }

        /*
         * Define what happens on collision with an entity.
         */
        public virtual void EntityCollision(Entity entity, GameTime gameTime)
        {
            if (!Colliding)
            {
                animTwo = defaultAnim;
                movementAnimation = "hit";
                collisionSeconds = 1;
                Colliding = true;
            }

            //RepelledFromEntity(10, entity);
        }

        /*
         * Define what happens on collision with an entity.
         */
        public virtual void SceneryCollision(Scenery scenery, GameTime gameTime)
        {
            if (!Colliding)
            {
                animTwo = defaultAnim;
                movementAnimation = "hit";
                collisionSeconds = 1;
                Colliding = true;
            }

            //RepelledFromScenery(8, scenery);
        }

        /*
         * Stop moving.
         */
        public void PauseMoving(GameTime gameTime)
        {
            if (!Pausing)
            {
                pauseSeconds = 0.5f;
                Pausing = true;
            }
        }

        /*
         * Wait for something.
         */
        public static float SecondsRemaining(GameTime gameTime, float seconds)
        {
            float deltaTime = GameLogic.GetTime(gameTime);
            seconds -= deltaTime;

            return seconds;
        }

        /*
         * Update entity Position.
         */
        public virtual void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            CheckIfFrozen(gameTime);
            WaitForCollisionSeconds(gameTime);
            WaitForPauseSeconds(gameTime);

            BeMoved(gameTime);
            //Movement(gameTime);

            CalculateSpeed(gameTime);
            AdjustXPosition(ImpassableArea);
            AdjustYPosition(ImpassableArea);
        }

        /*
         * Update the entity Position, animation state and skeleton.
         */
        public void UpdateAnimations(GameTime gameTime)
        {
            lastAnimation = movementAnimation;
            ChangeAnimation(movementAnimation);

            skeleton.X = position.X;
            skeleton.Y = position.Y;

            hitbox.Update(skeleton, true);
            animState.Update(GameLogic.GetTime(gameTime));
            skeleton.Update(GameLogic.GetTime(gameTime));
            animState.Apply(skeleton);

            // Update skeletal transformations
            skeleton.UpdateWorldTransform(Skeleton.Physics.Update);


        }

        /*
         * Update the hitbox after a collision.
         */
        public void UpdateHitbox(SkeletonBounds updatedHitbox)
        {
            hitbox = updatedHitbox;
        }

        /*
         * Set entity Position to the centre of the screen +/- any x,y axis adjustment.
         */
        public void SetPosition(float xAdjustment, float yAdjustment)
        {
            position = new Vector2(xAdjustment, yAdjustment);
        }

        /*
         * Get the skeleton.
         */
        public Skeleton GetSkeleton()
        {
            return skeleton;
        }

        /*
         * Get the hitbox.
         */
        public SkeletonBounds GetHitbox()
        {
            return hitbox;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}