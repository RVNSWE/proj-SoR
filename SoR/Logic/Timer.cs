namespace SoR.Logic
{
    internal class Timer
    {
        public bool CountDownComplete { get; set; }
        public float TimeElapsed { get; set; }

        public Timer()
        {
            CountDownComplete = false;
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
                CountDownComplete = false;
            }
            else CountDownComplete = true;
        }
    }
}