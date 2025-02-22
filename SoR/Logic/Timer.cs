using Microsoft.Xna.Framework;

namespace SoR.Logic
{
    internal class Timer
    {
        public bool TimerComplete { get; set; }
        public float TimeElapsed { get; set; }

        public Timer()
        {
            TimerComplete = false;
            TimeElapsed = 0;
        }

        /*
         * Countdown for events.
         */
        public void CountDown(float gameTime, Vector2 position, float interval)
        {
            float deltaTime = gameTime;

            if (TimeElapsed < interval)
            {
                TimeElapsed += deltaTime;
                TimerComplete = false;
            }
            else TimerComplete = true;
        }
    }
}
