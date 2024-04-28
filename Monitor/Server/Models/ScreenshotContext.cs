using Microsoft.EntityFrameworkCore;

namespace MonitorServer.Models
{
    public class ScreenshotContext : DbContext
    {
        public DbSet<Screenshot>? Screenshots { get; set; }

        public ScreenshotContext(DbContextOptions<ScreenshotContext> options)
            : base(options)
        {
        }
    }
}
