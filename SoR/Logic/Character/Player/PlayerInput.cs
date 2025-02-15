using Microsoft.Xna.Framework;

namespace SoR.Logic.Character.Player
{
    internal partial class Player : Entity
    {
        protected bool sitting;

        /*
         * Check whether the skin has changed.
         */
        public void CheckSkin()
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
         * Check for sit animation.
         * 
         * Currently Space to sit.
         */
        public void CheckSitting()
        {
            if ((keyboardInput.Key == "Space" ||
                gamePadInput.Button == "B") &&
                idle == true)
            {
                switch (isFacing)
                {
                    case "idledown":
                        if (!sitting)
                        {
                            movementAnimation = "sitdown";
                            isFacing = "sittingdown";
                            sitting = true;
                        }
                        else
                        {
                            movementAnimation = "idledown";
                            isFacing = "idledown";
                            sitting = false;
                        }
                        break;
                    case "idleup":
                        if (!sitting)
                        {
                            movementAnimation = "situp";
                            isFacing = "sittingup";
                            sitting = true;
                        }
                        else
                        {
                            movementAnimation = "idleup";
                            isFacing = "idleup";
                            sitting = false;
                        }
                        break;
                    case "idleleft":
                        if (!sitting)
                        {
                            movementAnimation = "sitleft";
                            isFacing = "sittingleft";
                            sitting = true;
                        }
                        else
                        {
                            movementAnimation = "idleleft";
                            isFacing = "idleleft";
                            sitting = false;
                        }
                        break;
                    case "idleright":
                        if (!sitting)
                        {
                            movementAnimation = "sitright";
                            isFacing = "sittingright";
                            sitting = true;
                        }
                        else
                        {
                            movementAnimation = "idleright";
                            isFacing = "idleright";
                            sitting = false;
                        }
                        break;
                }
                CheckSkin();
            }
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
                    movementAnimation = "runleft";
                    isFacing = "idleleft";
                    CheckSkin();
                    idle = false;
                    break;
                case 2:
                    MovementDirectionX(1);
                    movementAnimation = "runright";
                    isFacing = "idleright";
                    CheckSkin();
                    idle = false;
                    break;
                case 3:
                    direction.X = 0;
                    movementAnimation = lastAnimation;
                    break;
                case 4:
                    direction.X = 0;
                    if (!sitting)
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
                    movementAnimation = "runup";
                    isFacing = "idleup";
                    CheckSkin();
                    idle = false;
                    break;
                case 2:
                    MovementDirectionY(1);
                    movementAnimation = "rundown";
                    isFacing = "idledown";
                    CheckSkin();
                    idle = false;
                    break;
                case 3:
                    direction.Y = 0;
                    movementAnimation = lastAnimation;
                    break;
                case 4:
                    direction.Y = 0;
                    if (!sitting)
                    {
                        movementAnimation = isFacing;
                    }
                    CheckSkin();
                    break;
            }
        }
    }
}
