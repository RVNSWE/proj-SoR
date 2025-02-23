using Microsoft.Xna.Framework;
using SoR.Logic;
using System.Linq;

namespace SoR.Gameplay.Intro
{
    internal class Intro
    {
        private Timer timer;
        private Text text;
        public Vector2 TextPosition { get; set; }
        public string CurrentText { get; set; }
        public string CurrentSentence { get; set; }
        public int LineIndex { get; set; }
        public int CharIndex { get; set; }
        public bool NextLine {  get; set; }

        public Intro()
        {
            timer = new Timer();
            text = new Text();
            TextPosition = new Vector2();

            CharIndex = 0;
            LineIndex = 0;
            CurrentText = "";
            CurrentSentence = text.Items.ElementAt(LineIndex);
            NextLine = false;
        }

        /*
         * Start a new line.
         */
        public void StartNewLine()
        {
            if (LineIndex < text.Items.Length - 1)
            {
                LineIndex++;
                CharIndex = 0;
                CurrentText = "";
                CurrentSentence = text.Items.ElementAt(LineIndex);
            }
        }

        /*
         * Write text to the screen character by character.
         */
        public void WriteText(float gameTime, float textSize, Vector2 position, float interval)
        {
            if (CurrentText.Length < text.Items[LineIndex].Length)
            {
                timer.CountDown(gameTime, position, interval);
                NextLine = false;

                if (timer.TimerComplete)
                {
                    timer.TimeElapsed = 0;
                    CurrentText = CurrentText + text.Items[LineIndex].ElementAt(CharIndex).ToString();
                    CharIndex++;
                }
            }
            else
            {
                NextLine = true;
            }

            TextPosition = new Vector2(position.X - textSize, position.Y);
        }
    }
}