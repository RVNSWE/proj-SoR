using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace SoR.Hardware.Input
{
    /*
     * Handle keyboard input.
     */
    public class KeyboardInput
    {
        private KeyboardStateExtended keyState;
        private KeyboardStateExtended lastKeyState;
        private bool up;
        private bool down;
        private bool left;
        private bool right;
        private bool unpressedUp;
        private bool unpressedDown;
        private bool unpressedLeft;
        private bool unpressedRight;
        public bool CurrentInputDevice { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Key { get; set; }

        public KeyboardInput()
        {
            CurrentInputDevice = true;
            up = false;
            down = false;
            left = false;
            right = false;
            unpressedUp = false;
            unpressedDown = false;
            unpressedLeft = false;
            unpressedRight = false;
        }

        /*
         * Get input.
         */
        public void GetInput()
        {
            if (keyState.WasAnyKeyJustDown())
            {
                CurrentInputDevice = true;
            }
            else
            {
                CurrentInputDevice = false;
            }

            Key = CheckKeyInput();
            X = CheckXMovementDirection();
            Y = CheckYMovementDirection();

            if (!up && !down && !left && !right)
            {
                X = Y = 4;
            }
            if (right && left && down && up)
            {
                X = Y = 0;
            }
        }

        /*
         * Check keyboard input.
         * F4 = toggle fullscreen. F8 = save. F9 = load. Esc = exit. Enter = select menu item.
         * Space = sit. Escape = open start menu. Enter = OK.
         */
        public string CheckKeyInput()
        {
            Key = "none";
            keyState = KeyboardExtended.GetState(); // Get the current keyboard state

            if (keyState.IsKeyDown(Keys.F4) && !lastKeyState.IsKeyDown(Keys.F4))
            {
                Key = "F4";
            }
            if (keyState.IsKeyDown(Keys.F8) && !lastKeyState.IsKeyDown(Keys.F8))
            {
                Key = "F8";
            }
            if (keyState.IsKeyDown(Keys.F9) && !lastKeyState.IsKeyDown(Keys.F9))
            {
                Key = "F9";
            }
            if (keyState.IsKeyDown(Keys.Enter) && !lastKeyState.IsKeyDown(Keys.Enter))
            {
                Key = "Enter";
            }
            if (keyState.IsKeyDown(Keys.Space) & !lastKeyState.IsKeyDown(Keys.Space))
            {
                Key = "Space";
            }
            if (keyState.IsKeyDown(Keys.Escape) & !lastKeyState.IsKeyDown(Keys.Escape))
            {
                Key = "Escape";
            }
            if (keyState.IsKeyDown(Keys.Enter) & !lastKeyState.IsKeyDown(Keys.Enter))
            {
                Key = "Enter";
            }

            lastKeyState = keyState; // Get the previous keyboard state

            return Key;
        }

        /*
         * Check for and process keyboard x-axis movement inputs.
         */
        public int CheckXMovementDirection()
        {
            keyState = KeyboardExtended.GetState(); // Get the current keyboard state

            if (keyState.IsKeyDown(Keys.Left) ||
                keyState.IsKeyDown(Keys.A))
            {
                left = true;
            }
            if (keyState.IsKeyDown(Keys.Right) ||
                keyState.IsKeyDown(Keys.D))
            {
                right = true;
            }

            unpressedLeft =
                keyState.WasKeyReleased(Keys.Left) ||
                keyState.WasKeyReleased(Keys.A);

            unpressedRight =
                keyState.WasKeyReleased(Keys.Right) ||
                keyState.WasKeyReleased(Keys.D);

            if (unpressedLeft)
            {
                left = false;
            }
            if (unpressedRight)
            {
                right = false;
            }

            XMovement();
            lastKeyState = keyState; // Get the previous keyboard state

            return X;
        }

        /*
         * Check for and process keyboard y-axis movement inputs.
         */
        public int CheckYMovementDirection()
        {
            keyState = KeyboardExtended.GetState(); // Get the current keyboard state

            if (keyState.IsKeyDown(Keys.Up) ||
                keyState.IsKeyDown(Keys.W))
            {
                up = true;
            }
            if (keyState.IsKeyDown(Keys.Down) ||
                keyState.IsKeyDown(Keys.S))
            {
                down = true;
            }

            unpressedUp =
                keyState.WasKeyReleased(Keys.Up) ||
                keyState.WasKeyReleased(Keys.W);

            unpressedDown =
                keyState.WasKeyReleased(Keys.Down) ||
                keyState.WasKeyReleased(Keys.S);

            if (unpressedUp)
            {
                up = false;
            }
            if (unpressedDown)
            {
                down = false;
            }

            YMovement();
            lastKeyState = keyState; // Get the previous keyboard state

            return Y;
        }

        /*
         * Set x-axis movement.
         */
        public void XMovement()
        {
            X = 0;

            if (!(right && left && down && !up) ||
                !(right && left && up && !down))
            {
                if (left && !right &&
                    (!lastKeyState.IsKeyDown(Keys.Left) ||
                    !lastKeyState.IsKeyDown(Keys.A)))
                {
                    X = 1;
                }
                if (right && !left &&
                    (!lastKeyState.IsKeyDown(Keys.Right) ||
                    !lastKeyState.IsKeyDown(Keys.D)))
                {
                    X = 2;
                }
                if (unpressedLeft || unpressedRight)
                {
                    X = 3;
                }
                if (right && left && !down && !up)
                {
                    X = 4;
                }
            }
        }

        /*
         * Set y-axis movement.
         */
        public void YMovement()
        {
            Y = 0;

            if (!(down && up && right && !left) ||
                !(down && up && left && !right))
            {
                if (up && !down &&
                    (!lastKeyState.IsKeyDown(Keys.Up) ||
                    !lastKeyState.IsKeyDown(Keys.W)))
                {
                    Y = 1;
                }
                if (down && !up &&
                    (!lastKeyState.IsKeyDown(Keys.Down) ||
                    !lastKeyState.IsKeyDown(Keys.S)))
                {
                    Y = 2;
                }
                if (unpressedUp || unpressedDown)
                {
                    Y = 3;
                }
                if (down && up && !right && !left)
                {
                    Y = 4;
                }
            }
        }
    }
}