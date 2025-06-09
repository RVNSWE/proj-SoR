using Microsoft.Xna.Framework;
using SoR.Logic;
using System.Linq;

namespace SoR.Gameplay.Intro
{
    internal class Intro
    {
        private Timer timer;
        private Text text;
        private int charIndex;
        private int lineIndex;
        private bool newLine;
        public Vector2 TextPosition { get; set; }
        public string CurrentText { get; set; }
        public string CurrentSentence { get; set; }
        public float TextOpacity { get; set; }

        public Intro()
        {
            timer = new Timer();
            text = new Text();
            TextPosition = new Vector2();

            TextOpacity = 1f;
            charIndex = 0;
            lineIndex = 0;
            CurrentText = "";
            CurrentSentence = text.Line.ElementAt(lineIndex);
            newLine = false;
        }

        /*
         * Start a new line.
         */
        public void NextLine(float gameTime)
        {
            timer.CountDown(gameTime, 10f);
            TextOpacity -= gameTime * 0.15f;

            if (timer.CountDownComplete)
            {
                timer.TimeElapsed = 0;

                if (lineIndex < text.Line.Count - 1)
                {
                    TextOpacity = 1f;
                    charIndex = 0;
                    lineIndex++;
                    CurrentText = "";
                    CurrentSentence = text.Line.ElementAt(lineIndex);
                    newLine = false;
                }
            }
        }

        /*
         * Get the text to be written.
         */
        public void WriteText(float gameTime, float textSize, Vector2 position)
        {
            TextPosition = new Vector2(position.X - textSize, position.Y + 150);

            if (CurrentText.Length < text.Line[lineIndex].Length)
            {
                if (text.WriteTime.TryGetValue(text.Line.ElementAt(lineIndex), out float seconds))
                {
                    timer.CountDown(gameTime, seconds);
                }

                if (timer.CountDownComplete)
                {
                    timer.TimeElapsed = 0;
                    CurrentText = CurrentText + text.Line[lineIndex].ElementAt(charIndex).ToString();
                    charIndex++;
                }
            }
            else if (!newLine)
            {
                timer.CountDown(gameTime, 3f);

                if (timer.CountDownComplete)
                {
                    timer.TimeElapsed = 0;
                    newLine = true;
                }
            }
            if (newLine)
            {
                NextLine(gameTime);
            }
        }
    }
}