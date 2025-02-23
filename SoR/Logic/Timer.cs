namespace SoR.Logic
{
    internal class Timer
    {
        public bool TimerComplete { get; set; }
        public float TimeElapsed { get; set; }
        public float TimeUntil { get; set; }

        public Timer()
        {
            TimerComplete = false;
            TimeElapsed = 0;
        }

        /*
         * Count down for events.
         */
        public void CountDown(float gameTime, float seconds)
        {
            float deltaTime = gameTime;

            if (TimeElapsed < seconds)
            {
                TimeElapsed += deltaTime;
                TimerComplete = false;
            }
            else TimerComplete = true;
        }

        /*
         * Count up for events.
         */
        public void CountUp(float gameTime, float seconds)
        {
            float deltaTime = gameTime;

            if (TimeUntil < seconds)
            {
                TimeUntil -= deltaTime;
                TimerComplete = false;
            }
            else TimerComplete = true;
        }
    }
}