using SoR.Hardware.Graphics;
using SoR.Logic.Screens;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;

namespace SoR
{
    /*
     * The main game class, through which all other code runs. This project utilises Spine, MonoGame, and MonoGame.Extended.
     * 
     * -- FAO RECRUITERS (README) --
     * 
     * This project was created to teach myself C# and how to create a desktop application at a time when I could only write
     * simple console applications in Java and Python. I knew nothing about design patterns, architecture, Clean Code or SOLID
     * principles, and and just about everything in this project was developed organically through trial and error. There was no
     * planning involved, since the goal of this project was to learn to code, not to engineer.
     * 
     * It's a fully working 2D, top-down game engine with traversible maps, collision detection and interactions, character and
     * interactable object animations (no environmental animations yet), working menus, dynamically interchangeable X-Box
     * controller and keyboard support, and a save and load feature. There are no current known bugs, but it's only been tested
     * on Windows 10 and 11 on one desktop PC and one laptop.
     * 
     * -- -- -- -- -- -- -- -- -- --
     */

    /**************************************************************************************************************************
     * Copyright (c) 2024-2025, Katherine Town
     * 
     * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
     * following conditions are met:
     * 
     * 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following
     * disclaimer.
     * 
     * 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following
     * disclaimer in the documentation and/or other materials provided with the distribution.
     * 
     * 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products
     * derived from this software without specific prior written permission.
     * 
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS” AND ANY EXPRESS OR IMPLIED WARRANTIES,
     * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
     * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
     * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
     * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
     * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
     * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
     * 
     * (The 3-Clause BSD License: https://opensource.org/license/BSD-3-Clause)
     **************************************************************************************************************************/

    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private GraphicsSettings graphicsSettings;
        private MainGame game;
        private Screens screens;
        private int screenWidth;
        private int screenHeight;

        /*
         * Initialise content manager and graphics device.
         */
        public MainGame()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
        }

        /*
         * Initialise the game.
         */
        protected override void Initialize()
        {
            game = this;

            graphicsSettings = new GraphicsSettings(game, graphics, Window);

            screens = new Screens(game, GraphicsDevice);

            base.Initialize();
        }

        /*
         * Load game content.
         */
        protected override void LoadContent()
        {
            screens.LoadGame(game, GraphicsDevice, graphics);
        }

        /*
         * Update game elements.
         */
        protected override void Update(GameTime gameTime)
        {
            if (screens.ExitGame) Exit();
            KeyboardExtended.Update();
            UpdateResolution(graphicsSettings.CheckIfBorderlessToggled(graphics, Window));
            screens.UpdateGameState(gameTime, game, GraphicsDevice, graphics);

            base.Update(gameTime);
        }

        /*
         * Get the updated screen resolution if it changes.
         */
        public void UpdateResolution(Vector2 resolution)
        {
            if (screenWidth != (int)resolution.X ||
                screenHeight != (int)resolution.Y)
            {
                screenWidth = (int)resolution.X;
                screenHeight = (int)resolution.Y;

                screens.UpdateResolution(Window, screenWidth, screenHeight);

                graphics.ApplyChanges();
            }
        }

        /*
         * Draw game components to the screen.
         */
        protected override void Draw(GameTime gameTime)
        {
            screens.DrawGame(game, gameTime, GraphicsDevice, graphics);

            base.Draw(gameTime);
        }
    }
}