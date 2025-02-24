
using System.Collections.Generic;

namespace SoR.Gameplay.Intro
{
    internal class Text
    {
        public Dictionary<string, float> WriteTime { get; set; }
        public List<string> Line { get; set; }

        public Text()
        {
            WriteTime = new Dictionary<string, float>()
            {
                { "...", 0.8f },
                { "What are you?", 0.3f },
                { "I don't recognise you.", 0.1f },
                { "What are you even doing here?", 0.15f },
                { "I thought everyone was dead...?", 0.1f },
            };

            Line = [];

            foreach (string line in WriteTime.Keys)
            {
                Line.Add(line);
            }
        }
    }
}