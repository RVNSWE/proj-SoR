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
            CurrentSentence = text.Line.ElementAt(LineIndex);
            NextLine = false;
        }

        /*
         * Start a new line.
         */
        public void StartNewLine()
        {
            if (LineIndex < text.Line.Count - 1)
            {
                LineIndex++;
                CharIndex = 0;
                CurrentText = "";
                CurrentSentence = text.Line.ElementAt(LineIndex);
            }
        }

        /*
         * Get the text to be written.
         */
        public void GetText(float gameTime, float textSize, Vector2 position)
        {
            if (CurrentText.Length < text.Line[LineIndex].Length)
            {
                if (text.WriteTime.TryGetValue(text.Line.ElementAt(LineIndex), out float seconds))
                {
                    timer.CountDown(gameTime, seconds);
                }

                NextLine = false;

                if (timer.TimerComplete)
                {
                    timer.TimeElapsed = 0;
                    CurrentText = CurrentText + text.Line[LineIndex].ElementAt(CharIndex).ToString();
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