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
            float strWidth,
            float conWidth,
            float agiWidth,
            float intWidth
            )
        {
            int statBarYOffset = 250;
            int strXOffset = 350;
            int conXOffset = 200;
            int agiXOffset = 50;
            int intXOffset = 100;

            string strLabel = "STR: ";
            Vector2 strBarPosition = new(position.X - strXOffset, position.Y - statBarYOffset);
            Vector2 strTextPosition = new(strBarPosition.X - TextSize(strLabel).X, strBarPosition.Y);
            string conLabel = "CON: ";
            Vector2 conBarPosition = new(position.X - conXOffset, position.Y - statBarYOffset);
            Vector2 conTextPosition = new(conBarPosition.X - TextSize(conLabel).X, conBarPosition.Y);
            string agiLabel = "AGI: ";
            Vector2 agiBarPosition = new(position.X - agiXOffset, position.Y - statBarYOffset);
            Vector2 agiTextPosition = new(agiBarPosition.X - TextSize(agiLabel).X, agiBarPosition.Y);
            string intLabel = "INT: ";
            Vector2 intBarPosition = new (position.X + intXOffset, position.Y - statBarYOffset);
            Vector2 intTextPosition = new (intBarPosition.X - TextSize(intLabel).X, intBarPosition.Y);

            StartDrawingSpriteBatch(camera);
            DrawText(strTextPosition, strLabel, 0.6f);
            DrawStatBar(0, strBarPosition, strWidth);
            DrawText(conTextPosition, conLabel, 0.6f);
            DrawStatBar(1, conBarPosition, conWidth);
            DrawText(agiTextPosition, agiLabel, 0.6f);
            DrawStatBar(2, agiBarPosition, agiWidth);
            DrawText(intTextPosition, intLabel, 0.6f);
            DrawStatBar(3, intBarPosition, intWidth);
            FinishDrawingSpriteBatch();
        }

        /*
         * Draw a given UI stat bar with the given position and width.
         */
        public void DrawStatBar(int stat, Vector2 position, float width)
        {
            Vector2 scale = new(width, statHeight);
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