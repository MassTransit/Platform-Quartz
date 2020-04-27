namespace Platform.QuartzService
{
    public class QuartzOptions
    {
        public QuartzOptions()
        {
            ConnectionString =
                "Server=tcp:localhost;Database=quartznet;Persist Security Info=False;User ID=sa;Password=Quartz!DockerP4ss;Encrypt=False;TrustServerCertificate=True;";
        }

        public string Queue { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int? ConcurrentMessageLimit { get; set; }
        public int? ThreadCount { get; set; }
        public string InstanceName { get; set; }
        public string TablePrefix { get; set; }
    }
}