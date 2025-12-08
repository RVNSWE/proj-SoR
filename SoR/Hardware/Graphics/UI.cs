using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SoR.Hardware.Graphics
{
    internal partial class Render
    {
        /*
         * Draw the curtain at the given position, width, height and opacity.
         */
        public void DrawCurtain(Vector2 position, OrthographicCamera camera, int width, int height, float fadeAlpha = 1f)
        {
            Vector2 scale = new Vector2(width, height);
            Vector2 adjustedPosition = new Vector2(position.X - (width / 2), position.Y - (height / 2));

            StartDrawingSpriteBatch(camera);
            spriteBatch.Draw(
                Curtain,
                adjustedPosition,
                null,
                Color.White * fadeAlpha,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
            FinishDrawingSpriteBatch();
        }

        /*
         * Draw a UI stat bar with the given position and width.
         */
        public void DrawStatBar(Vector2 position, OrthographicCamera camera, float width)
        {
            int height = 5;
            Vector2 scale = new Vector2(width, height);

            StartDrawingSpriteBatch(camera);
            spriteBatch.Draw(
                Str,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
            FinishDrawingSpriteBatch();
        }
    }
}