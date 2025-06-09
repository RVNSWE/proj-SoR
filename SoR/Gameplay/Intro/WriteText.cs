using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SoR.Logic;

namespace SoR.Gameplay.Intro
{
    internal class Text
    {
        private Timer timer;
        private int charIndex;
        private int lineIndex;
        private bool newLine;
        public Vector2 TextPosition { get; set; }
        public string CurrentText { get; set; }
        public string CurrentSentence { get; set; }
        public float TextOpacity { get; set; }
        public Dictionary<string, float> WriteTime { get; set; }
        public List<string> Line { get; set; }

        public Text()
        {
            timer = new Timer();
            TextPosition = new Vector2();

            TextOpacity = 1f;
            charIndex = 0;
            lineIndex = 0;
            CurrentText = "";
            CurrentSentence = Line.ElementAt(lineIndex);
            newLine = false;

            WriteTime = new Dictionary<string, float>()
            {
                { "...", 0.8f },
                { "What are you?", 0.3f }
            };

            Line = [];

            foreach (string line in WriteTime.Keys)
            {
                Line.Add(line);
            }
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

                if (lineIndex < Line.Count - 1)
                {
                    TextOpacity = 1f;
                    charIndex = 0;
                    lineIndex++;
                    CurrentText = "";
                    CurrentSentence = Line.ElementAt(lineIndex);
                    newLine = false;
                }
            }
        }

        /*
         * Get the text to be written.
         */
        public void GetText(float gameTime, float textSize, Vector2 position)
        {
            TextPosition = new Vector2(position.X - textSize, position.Y + 150);

            if (CurrentText.Length < Line[lineIndex].Length)
            {
                if (WriteTime.TryGetValue(Line.ElementAt(lineIndex), out float seconds))
                {
                    timer.CountDown(gameTime, seconds);
                }

                if (timer.CountDownComplete)
                {
                    timer.TimeElapsed = 0;
                    CurrentText = CurrentText + Line[lineIndex].ElementAt(charIndex).ToString();
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