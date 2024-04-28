using System.ComponentModel.DataAnnotations;

namespace MonitorServer.Models
{
    public class Screenshot
    {
        public string imageUrl { get; set; }
        public DateTime timestamp { get; set; }
        [Key]
        public string username { get; set; }

        public Screenshot(string imageUrl, DateTime timestamp, string username)
        {
            this.imageUrl = imageUrl;
            this.timestamp = timestamp;
            this.username = username;
        }
    }
}
