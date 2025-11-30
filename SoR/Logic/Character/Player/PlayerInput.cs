using Microsoft.Xna.Framework;

namespace SoR.Logic.Character.Player
{
    internal partial class Player : Entity
    {
        /*
         * Check whether the skin has changed.
         */
        public void CheckSkin()
        {
            switch (isFacing)
            {
                case "U_idle":
                    skeleton.SetSkin(skeletonData.FindSkin("up"));
                    break;
                case "D_idle":
                    skeleton.SetSkin(skeletonData.FindSkin("down"));
                    break;
                case "L_idle":
                    skeleton.SetSkin(skeletonData.FindSkin("left"));
                    break;
                case "R_idle":
                    skeleton.SetSkin(skeletonData.FindSkin("right"));
                    break;
            }

            skeleton.SetSlotsToSetupPose();
            animState.Apply(skeleton);
        }

        /*
         * Check for sit animation.
         * 
         * Currently Space to sit.
         */
        public void CheckSitting()
        {
            /*if ((keyboardInput.Key == "Space" ||
                gamePadInput.Button == "B") &&
                idle == true)
            {
                switch (isFacing)
                {
                    case "D_idle":
                        movementAnimation = "D_sit";
                        isFacing = "D_sitting";
                        break;
                    case "D_sitting":
                        movementAnimation = "D_stand";
                        isFacing = "D_idle";
                        break;
                    case "U_idle":
                        movementAnimation = "U_sit";
                        isFacing = "U_sitting";
                        break;
                    case "U_sitting":
                        movementAnimation = "U_stand";
                        isFacing = "U_idle";
                        break;
                    case "L_idle":
                        movementAnimation = "L_sit";
                        isFacing = "L_sitting";
                        break;
                    case "L_sitting":
                        movementAnimation = "L_stand";
                        isFacing = "L_idle";
                        break;
                    case "R_idle":
                        movementAnimation = "R_sit";
                        isFacing = "R_sitting";
                        break;
                    case "R_sitting":
                        movementAnimation = "R_stand";
                        isFacing = "R_idle";
                        break;
                }
                CheckSkin();
            }*/
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
                    movementAnimation = isFacing; // Set idle animation according to direction player is facing
                    idle = true; // Idle is now playing
                    CheckSkin();
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
                    movementAnimation = "L_run";
                    isFacing = "L_idle";
                    CheckSkin();
                    idle = false;
                    break;
                case 2:
                    MovementDirectionX(1);
                    movementAnimation = "R_run";
                    isFacing = "R_idle";
                    CheckSkin();
                    idle = false;
                    break;
                case 3:
                    direction.X = 0;
                    movementAnimation = lastAnimation;
                    break;
                case 4:
                    direction.X = 0;
                    if (movementAnimation != lastAnimation)
                    {
                        movementAnimation = isFacing;
                    }
                    CheckSkin();
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
                    movementAnimation = "U_run";
                    isFacing = "U_idle";
                    CheckSkin();
                    idle = false;
                    break;
                case 2:
                    MovementDirectionY(1);
                    movementAnimation = "D_run";
                    isFacing = "D_idle";
                    CheckSkin();
                    idle = false;
                    break;
                case 3:
                    direction.Y = 0;
                    movementAnimation = lastAnimation;
                    break;
                case 4:
                    direction.Y = 0;
                    if (movementAnimation != lastAnimation)
                    {
                        movementAnimation = isFacing;
                    }
                    CheckSkin();
                    break;
            }
        }
    }
}
