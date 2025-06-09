namespace SoR.Logic
{
    internal class Timer
    {
        public bool CountDownComplete { get; set; }
        public bool CountDownTwoComplete { get; set; }
        public float TimeElapsed { get; set; }
        public float TimeElapsedTwo { get; set; }

        public Timer()
        {
            CountDownComplete = false;
            CountDownTwoComplete = false;
            TimeElapsed = 0;
            TimeElapsedTwo = 0;
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

        /*
         * Count up for events.
         */
        public void SecondCountDown(float gameTime, float seconds)
        {
            float deltaTime = gameTime;

            if (TimeElapsedTwo < seconds)
            {
                TimeElapsedTwo += deltaTime;
                CountDownTwoComplete = false;
            }
            else CountDownTwoComplete = true;
        }
    }
}