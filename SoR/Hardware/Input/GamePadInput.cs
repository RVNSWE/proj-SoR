using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace SoR.Hardware.Input
{
    /*
     * Game pad input.
     */
    public class GamePadInput
    {
        private GamePadListener gamePadListener;
        private GamePadState gamePadState;
        private GamePadState lastGamePadState;
        private GamePadCapabilities gamePadCapabilities;
        public string Button { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GamePadInput()
        {
            gamePadListener = new GamePadListener();
            gamePadCapabilities = GamePad.GetCapabilities(PlayerIndex.One);
        }

        /*
         * Get input.
         */
        public void GetInput()
        {
            Button = CheckButtonInput();
            X = CheckXMoveInput();
            Y = CheckYMoveInput();
        }

        /*
         * Check button input.
         * A = load. B = save. Start = open start menu. Back = toggle fullscreen. DPad = navigation. B = change skin.
         */
        public string CheckButtonInput()
        {
            Button = "none";

            if (gamePadCapabilities.IsConnected) // If the gamepad is connected
            {
                gamePadState = GamePad.GetState(PlayerIndex.One); // Get the current gamepad state

                if (gamePadState.Buttons.B == ButtonState.Pressed && lastGamePadState.Buttons.B != ButtonState.Pressed)
                {
                    Button = "B";
                }
                if (gamePadState.Buttons.A == ButtonState.Pressed && lastGamePadState.Buttons.A != ButtonState.Pressed)
                {
                    Button = "A";
                }
                if (gamePadState.Buttons.Start == ButtonState.Pressed && lastGamePadState.Buttons.Start != ButtonState.Pressed)
                {
                    Button = "Start";
                }
                if (gamePadState.Buttons.Back == ButtonState.Pressed && lastGamePadState.Buttons.Back != ButtonState.Pressed)
                {
                    Button = "Back";
                }
                if (gamePadState.DPad.Up == ButtonState.Pressed &&
                    lastGamePadState.DPad.Up != ButtonState.Pressed)
                {
                    Button = "Up";
                }
                if (gamePadState.DPad.Down == ButtonState.Pressed &&
                    lastGamePadState.DPad.Down != ButtonState.Pressed)
                {
                    Button = "Down";
                }

                lastGamePadState = gamePadState; // Get the previous gamepad state
            }

            return Button;
        }

        /*
         * Check for and process gamepad input.
         */
        public int CheckXMoveInput()
        {
            X = 0;

            if (gamePadCapabilities.IsConnected) // If the gamepad is connected
            {
                gamePadState = GamePad.GetState(PlayerIndex.One); // Get the current gamepad state

                if (gamePadState.ThumbSticks.Left.X < -0.5f)
                {
                    X = 1;
                }
                else if (gamePadState.ThumbSticks.Left.X > 0.5f)
                {
                    X = 2;
                }

                if (gamePadState.ThumbSticks.Left.X !< -0.5f &&
                    gamePadState.ThumbSticks.Left.X !> 0.5f)
                {
                    X = 0;
                }

                lastGamePadState = gamePadState;
            }

            return X;
        }

        /*
         * Check for and process gamepad input.
         */
        public int CheckYMoveInput()
        {
            Y = 0;

            if (gamePadCapabilities.IsConnected) // If the gamepad is connected
            {
                gamePadState = GamePad.GetState(PlayerIndex.One); // Get the current gamepad state

                if (gamePadState.ThumbSticks.Left.Y < -0.5f)
                {
                    Y = 2;
                }
                else if (gamePadState.ThumbSticks.Left.Y > 0.5f)
                {
                    Y = 1;
                }

                if (gamePadState.ThumbSticks.Left.Y !< -0.5f &&
                    gamePadState.ThumbSticks.Left.Y !> 0.5f)
                {
                    Y = 0;
                }

                lastGamePadState = gamePadState;
            }

            return Y;
        }
    }
}
