namespace APICatalogo.RateLimitOptions
{
    public class MyRateLimitOptions
    {
        public const string MyRateLimit = "MyRateLimit";
        public int PermitLimit { get; set; }
        public int Window { get; set; }
        public int ReplenishmentPeriod { get; set; }
        public int QueueLimit { get; set; }
        public int SegmentsPerWindow { get; set; }
        public int TokenLimit { get; set; }
        public int TokenLimit2 { get; set; }
        public int TokensPerPeriod { get; set; }
        public bool AutoReplenishment { get; set; }
    }
}
