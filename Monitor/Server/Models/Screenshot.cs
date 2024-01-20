namespace MonitorServer.Models
{
    public class Screenshot
    {
        public string imageUrl { get; set; }
        public DateTime timestamp { get; set; }

        public Screenshot(string imageUrl, DateTime timestamp)
        {
            this.imageUrl = imageUrl;
            this.timestamp = timestamp;
        }
    }
}
