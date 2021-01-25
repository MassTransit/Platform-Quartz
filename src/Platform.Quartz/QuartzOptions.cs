namespace Platform.QuartzService
{
    public class QuartzOptions
    {
        public string Queue { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int? ConcurrentMessageLimit { get; set; }
        public int? ThreadCount { get; set; }
        public string InstanceName { get; set; }
        public string TablePrefix { get; set; }
        public bool? Clustered { get; set; }
        public string DriverDelegateType { get; set; }
    }
}
