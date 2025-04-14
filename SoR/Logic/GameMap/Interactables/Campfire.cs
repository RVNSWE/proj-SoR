using Microsoft.Xna.Framework.Graphics;
using Spine;
using SoR.Logic.Character;

namespace SoR.Logic.GameMap.Interactables
{
    /*
     * Stores information unique to Campfire.
     */
    internal class Campfire : Scenery
    {
        public Campfire(GraphicsDevice GraphicsDevice)
        {
            // Load texture atlas and attachment loader
            atlas = new Atlas(Globals.GetResourcePath("Content\\SoR Resources\\Locations\\Interactables\\Campfire\\templecampfire.atlas"), new XnaTextureLoader(GraphicsDevice));
            atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
            json = new SkeletonJson(atlasAttachmentLoader);
            json.Scale = 0.5f;

            // Initialise skeleton json
            skeletonData = json.ReadSkeletonData(Globals.GetResourcePath("Content\\SoR Resources\\Locations\\Interactables\\Campfire\\skeleton.json"));
            skeleton = new Skeleton(skeletonData);

            // Set the skin
            skeleton.SetSkin(skeletonData.FindSkin("default"));

            // Setup animation
            animStateData = new AnimationStateData(skeleton.Data);
            animState = new AnimationState(animStateData);
            animState.Apply(skeleton);
            animStateData.DefaultMix = 0.1f;

            // Set the "fidle" animation on track 1 and leave it looping forever
            trackEntry = animState.SetAnimation(0, "idle", true);

            // Create hitbox
            slot = skeleton.FindSlot("hitbox");
            hitboxAttachment = skeleton.GetAttachment("hitbox", "hitbox");
            slot.Attachment = hitboxAttachment;
            skeleton.SetAttachment("hitbox", "hitbox");

            hitbox = new SkeletonBounds();
        }

        /*
         * Placeholder for Campfire animation changes.
         */
        public override void ChangeAnimation(string eventTrigger) { }

        /*
         * Placeholder for performing an interaction.
         */
        public override void InteractWith(Entity entity) { }

        /*
         * Define what happens on collision with an entity.
         */
        /*public override void Collision(Entity entity, GameTime gameTime)
        {
            entity.SceneryCollision(this, gameTime);
            //entity.ChangeAnimation("hit");
        }*/
    }
}