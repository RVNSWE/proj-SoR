using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace SoR.Hardware.Graphics
{
    internal partial class Render
    {
        /*
         * Distort text to shake slightly.
         */
        public void DrawDistortSpriteBatch(SpriteFont font, Vector2 position)
        {
            float distortAmp = new Random().Next(2);
            float distortFreq = new Random().Next(2);
            string text = "Who are you?";

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
    }
}
