using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SoR.Hardware.Graphics
{
    internal partial class Render
    {
        /*
         * 0 = Str (stress)
         * 1 = Con (conviction)
         * 2 = Agi (agitation)
         * 3 = Int (initiation energy)
         */
        enum StatType
        {
            Str,
            Con,
            Agi,
            Int
        }

        /*
         * Draw the whole UI to the screen.
         */
        public void DrawUI(
            Vector2 position,
            OrthographicCamera camera,
            float intWidth
            )
        {
            string strLabel = "STR: ";
            Vector2 strBarPosition = new(position.X - 350, position.Y - 250);
            Vector2 strTextPosition = new(strBarPosition.X - TextSize(strLabel).X, strBarPosition.Y - 2.5f);
            string conLabel = "CON: ";
            Vector2 conBarPosition = new(position.X - 100, position.Y - 250);
            Vector2 conTextPosition = new(conBarPosition.X - TextSize(conLabel).X, conBarPosition.Y - 2.5f);
            string agiLabel = "AGI: ";
            Vector2 agiBarPosition = new(position.X - 350, position.Y - 200);
            Vector2 agiTextPosition = new(agiBarPosition.X - TextSize(agiLabel).X, agiBarPosition.Y - 2.5f);
            string intLabel = "INT: ";
            Vector2 intBarPosition = new (position.X - 100, position.Y - 200);
            Vector2 intTextPosition = new (intBarPosition.X - TextSize(intLabel).X, intBarPosition.Y - 2.5f);

            StartDrawingSpriteBatch(camera);
            DrawText(intTextPosition, intLabel, 0.6f);
            DrawStatBar(3, intBarPosition, camera, intWidth);
            DrawText(intTextPosition, intLabel, 0.6f);
            DrawStatBar(3, intBarPosition, camera, intWidth);
            DrawText(intTextPosition, intLabel, 0.6f);
            DrawStatBar(3, intBarPosition, camera, intWidth);
            DrawText(intTextPosition, intLabel, 0.6f);
            DrawStatBar(3, intBarPosition, camera, intWidth);
            FinishDrawingSpriteBatch();
        }

        /*
         * Draw a given UI stat bar with the given position and width.
         */
        public void DrawStatBar(int stat, Vector2 position, OrthographicCamera camera, float width)
        {
            int height = 5;
            Vector2 scale = new(width, height);
            StatType statType = (StatType)stat;
            Texture2D drawStat = Curtain;

            switch (statType)
            {
                case StatType.Str:
                    drawStat = Str;
                    break;
                case StatType.Con:
                    drawStat = Con;
                    break;
                case StatType.Agi:
                    drawStat = Agi;
                    break;
                case StatType.Int:
                    drawStat = Int;
                    break;
            }

            spriteBatch.Draw(
                drawStat,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }

        /*
         * Draw the curtain at the given position, width, height and opacity.
         */
        public void DrawCurtain(Vector2 position, OrthographicCamera camera, int width, int height, float fadeAlpha = 1f)
        {
            Vector2 scale = new (width, height);
            Vector2 adjustedPosition = new (position.X - (width / 2), position.Y - (height / 2));

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
    }
}