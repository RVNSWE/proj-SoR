using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoR.Logic.Character;
using SoR.Logic.GameMap;
using System;
using System.Linq;
using static System.Windows.Forms.AxHost;

namespace SoR.Hardware.Graphics
{
    internal partial class Render
    {
        /*
         * Draw SpriteBatch for entities.
         */
        public void DrawEntitySpriteBatch(Entity entity)
        {
            spriteBatch.DrawString(
                font,
                "HP: " + entity.GetHitPoints() + "\nEnergy: " + entity.GetStatValue("INT"),
                new Vector2(entity.GetPosition().X - 30, entity.GetPosition().Y + 30),
                Color.BlueViolet);
        }

        /*
         * Draw SpriteBatch for scenery.
         */
        public void DrawScenerySpriteBatch(Scenery scenery)
        {
            spriteBatch.DrawString(
                font,
                "X: " + scenery.GetPosition().X + " Y: " + scenery.GetPosition().Y,
                new Vector2(scenery.GetPosition().X - 80, scenery.GetPosition().Y + 50),
                Color.BlueViolet);
        }

        /*
         * Draw the text for the main menu.
         */
        public void MenuText(string menuItem, Vector2 position, Color colour, float scale)
        {
            // Entity text
            spriteBatch.DrawString(
                font,
                menuItem,
                new Vector2(position.X, position.Y),
                colour,
                0,
                new Vector2(0, 0),
                scale,
                SpriteEffects.None,
                0);
        }

        /*
         * Draw general text.
         */
        public void DrawText(Vector2 position, string text, float scale, float fadeAlpha = 1f)
        {
            spriteBatch.DrawString(
            font,
            text,
            position,
            Color.White * fadeAlpha,
            0,
            new Vector2(0, 0),
            scale,
            SpriteEffects.None,
            0);
        }

        /*
         * Distort text to shake slightly.
         */
        public void DrawDistortedText(Vector2 position, string text)
        {
            float distortAmp = new Random().Next(2);
            float distortFreq = new Random().Next(2);

            for (int i = 0; i < text.Length; i++)
            {
                float charX = position.X + new Random().Next(2) + i * font.MeasureString(text.ElementAt(i).ToString()).X;
                float charY = position.Y + (float)Math.Sin(charX * distortFreq) * distortAmp;

                spriteBatch.DrawString(
                font,
                text[i].ToString(),
                new Vector2(charX - font.MeasureString(text).X / 2, charY),
                Color.White);
            }
        }

        /*
         * Used for measuring the current string for positioning on the screen.
         */
        public Vector2 TextSize(string text)
        {
            Vector2 textSize = new (0, 0);

            textSize.X = font.MeasureString(text).X / 2;
            textSize.Y = font.MeasureString(text).Y / 2;

            return textSize;
        }
    }
}