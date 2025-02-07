using Microsoft.Xna.Framework;

namespace SoR.Logic.Character.Player
{
    internal partial class Player : Entity
    {
        /*
         * Check whether the skin has changed.
         */
        public void SkinChange()
        {
            switch (isFacing)
            {
                case "idleup":
                    skeleton.SetSkin(skeletonData.FindSkin("up"));
                    break;
                case "idledown":
                    skeleton.SetSkin(skeletonData.FindSkin("down"));
                    break;
                case "idleleft":
                    skeleton.SetSkin(skeletonData.FindSkin("left"));
                    break;
                case "idleright":
                    skeleton.SetSkin(skeletonData.FindSkin("right"));
                    break;
            }

            skeleton.SetSlotsToSetupPose();
            animState.Apply(skeleton);
        }

        /*
         * Check whether player is idle.
         */
        public void CheckIdle()
        {
            if ((keyboardInput.CurrentInputDevice && 
                keyboardInput.X == 0 && keyboardInput.Y == 0) ||
                (!keyboardInput.CurrentInputDevice &&
                gamePadInput.X == 0 && gamePadInput.Y == 0))
            {
                if (!idle) // If idle animation is not currently playing
                {
                    idle = true; // Idle is now playing
                    movementAnimation = isFacing; // Set idle animation according to direction player is facing
                    SkinChange();
                }

                if (CountDistance == 0)
                {
                    direction = Vector2.Zero;
                }
            }
        }

        /*
         * Process keyboard and gamepad x-axis movement inputs.
         */
        public void ProcessXMovementInput(int x)
        {
            switch (x)
            {
                case 0:
                    if (CountDistance == 0)
                    {
                        direction.X = 0;
                    }
                    break;
                case 1:
                    MovementDirectionX(-1);
                    idle = false;
                    movementAnimation = "runleft";
                    isFacing = "idleleft";
                    SkinChange();
                    break;
                case 2:
                    MovementDirectionX(1);
                    idle = false;
                    movementAnimation = "runright";
                    isFacing = "idleright";
                    SkinChange();
                    break;
                case 3:
                    direction.X = 0;
                    movementAnimation = lastAnimation;
                    break;
                case 4:
                    direction.X = 0;
                    movementAnimation = "idledown";
                    isFacing = "idledown";
                    SkinChange();
                    break;
            }
        }

        /*
         * Process keyboard and gamepad y-axis movement inputs.
         */
        public void ProcessYMovementInput(int y)
        {
            switch (y)
            {
                case 0:
                    if (CountDistance == 0)
                    {
                        direction.Y = 0;
                    }
                    break;
                case 1:
                    MovementDirectionY(-1);
                    movementAnimation = "runup";
                    isFacing = "idleup";
                    SkinChange();
                    idle = false;
                    break;
                case 2:
                    MovementDirectionY(1);
                    movementAnimation = "rundown";
                    isFacing = "idledown";
                    SkinChange();
                    idle = false;
                    break;
                case 3:
                    direction.Y = 0;
                    movementAnimation = lastAnimation;
                    break;
                case 4:
                    direction.Y = 0;
                    movementAnimation = isFacing;
                    break;
                case 5:
                    direction.Y = 0;
                    break;
            }
        }
    }
}
