using SoR.Logic.GameMap;
using Microsoft.Xna.Framework;
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
     * Common functions and fields for player and non-player characters.
     */
    public partial class Entity
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
        protected Vector2 prevPosition;
        protected Vector2 position;
        protected bool inMotion;
        protected string prevTrigger;
        protected string animOne;
        protected string animTwo;
        protected string isFacing;
        public List<Rectangle> ImpassableArea { get; protected set; }
        public bool Player { get; set; }
        public string Type { get; set; }
        public int HitPoints { get; set; }
        public int Speed { get; set; }
        public string Skin { get; set; }

        /*
         * Placeholder function for dealing damage.
         */
        public void TakeDamage(int damage)
        {
            HitPoints -= damage;
        }

        /*
         * Check if moving.
         */
        public bool IsMoving()
        {
            return inMotion;
        }

        /*
         * Start moving and switch to running animation.
         */
        public void StartMoving()
        {
            inMotion = true;
            ChangeAnimation("run");
        }

        /*
         * Stop moving.
         */
        public void StopMoving()
        {
            inMotion = false;
            ChangeAnimation("idle");
            position = prevPosition;
        }

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
                prevTrigger = animOne = reaction = eventTrigger;
                animTwo = "idle";
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
                        animState.AddAnimation(0, animOne, true, -trackEntry.TrackComplete);
                        break;
                    case 2:
                        animState.SetAnimation(0, animOne, false);
                        trackEntry = animState.AddAnimation(0, animTwo, true, 0);
                        break;
                    case 3:
                        if (trackEntry != null) // If there's a queue then buttons are being mashed, so just clear it and set next.
                        {
                            animState.SetAnimation(0, animOne, true);
                        }
                        else // Otherwise, add next to start on frame current anim finished on.
                        {
                            animState.AddAnimation(0, animOne, true, -trackEntry.TrackTime);
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

                AdjustPosition(gameTime, ImpassableArea);

                CountDistance--;
            }
        }

        /*
         * Define what happens on collision with an entity.
         */
        public virtual void EntityCollision(Entity entity, GameTime gameTime)
        {
            entity.TakeDamage(1);
            entity.ChangeAnimation("hit");
            RepelledFromEntity(10, entity);
        }

        /*
         * Define what happens on collision with an entity.
         */
        public virtual void SceneryCollision(Scenery scenery, GameTime gameTime)
        {
            TakeDamage(1);
            ChangeAnimation("hit");
            RepelledFromScenery(8, scenery);
        }

        /*
         * Update entity position.
         */
        public virtual void UpdatePosition(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            FrozenTimer(gameTime);

            if (!Frozen)
            {
                BeMoved(gameTime);
                NonPlayerMovement(gameTime);

                AdjustPosition(gameTime, ImpassableArea);

                DirectionReversed = false;
            }
        }

        /*
         * Update the hitbox after a collision.
         */
        public void UpdateHitbox(SkeletonBounds updatedHitbox)
        {
            hitbox = updatedHitbox;
        }

        /*
         * Update the entity position, animation state and skeleton.
         */
        public virtual void UpdateAnimations(GameTime gameTime)
        {
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
         * Set entity position to the centre of the screen +/- any x,y axis adjustment.
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

        /*
         * Get the current hitpoints.
         */
        public int GetHitPoints()
        {
            return HitPoints;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}