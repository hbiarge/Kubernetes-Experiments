namespace Shared.HealthChecks
{
    public static class Constants
    {
        public static class Health
        {
            public const string LivenessTag = "Liveness";
            public const string ReadinessTag = "Readiness";
            public const string LivenessCheckName = "SelfLive";
            public const string ReadynessCheckName = "SelfReady";
        }
    }
}