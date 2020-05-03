namespace Shared.HealthChecks
{
    public class HealthCheckState
    {
        public bool IsLive { get; private set; } = true;

        public bool IsReady { get; private set; } = true;

        public void SetNotLive()
        {
            if (IsLive)
            {
                IsLive = false;
            }
        }

        public void SetLive()
        {
            if (!IsLive)
            {
                IsLive = true;
            }
        }

        public void SetNotReady()
        {
            if (IsReady)
            {
                IsReady = false;
            }
        }

        public void SetReady()
        {
            if (!IsReady)
            {
                IsReady = true;
            }
        }
    }
}