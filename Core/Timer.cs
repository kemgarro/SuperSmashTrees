namespace SuperSmashTrees.Core
{
    public class Timer
    {
        public float TotalTime { get; private set; }
        public float RemainingTime { get; private set; }
        public bool TimeOver => RemainingTime <= 0;

        public Timer(float totalSeconds)
        {
            TotalTime = totalSeconds;
            RemainingTime = totalSeconds;
        }

        public void Update(float delta)
        {
            if (RemainingTime > 0)
                RemainingTime -= delta;
        }

        public string GetFormattedTime()
        {
            int seconds = (int)MathF.Max(RemainingTime, 0);
            int minutes = seconds / 60;
            seconds %= 60;
            return $"{minutes:D2}:{seconds:D2}";
        }

        public void Reset()
        {
            RemainingTime = TotalTime;
        }
    }
}
