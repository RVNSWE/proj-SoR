using Microsoft.Xna.Framework.Graphics;
using Spine;
using SoR.Logic.Character;

namespace SoR.Logic.GameMap.Interactables
{
    /*
     * Stores information unique to Ship.
     */
    internal class Ship : Scenery
    {
        // Ship attributes:
        private int fuel;
        private int energy;
        private int hullHp;
        private int engineHp;
        private int shieldHp;
        private int weaponHp;
        private int cloakHp;

        public Ship(GraphicsDevice GraphicsDevice)
        {
            // Load texture atlas and attachment loader
            atlas = new Atlas(Globals.GetResourcePath("Content\\SoR Resources\\Locations\\Interactables\\Campfire\\templecampfire.atlas"), new XnaTextureLoader(GraphicsDevice));
            atlasAttachmentLoader = new AtlasAttachmentLoader(atlas);
            json = new SkeletonJson(atlasAttachmentLoader);

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

            // Set the "idle" animation on track 1 and leave it looping forever
            trackEntry = animState.SetAnimation(0, "idle", true);

            // Create hitbox
            slot = skeleton.FindSlot("hitbox");
            hitboxAttachment = skeleton.GetAttachment("hitbox", "hitbox");
            slot.Attachment = hitboxAttachment;
            skeleton.SetAttachment("hitbox", "hitbox");

            hitbox = new SkeletonBounds();

            id = 0;
            fuel = 100;
            energy = 100;
            hullHp = 100;
            engineHp = 100;
            shieldHp = 100;
            weaponHp = 100;
            cloakHp = 100;
        }

        /*
         * Placeholder for animation changes.
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