using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskFlow.Models.Users> Users { get; set; }
        public DbSet<TaskFlow.Models.Project> Project { get; set; }
        public DbSet<TaskFlow.Models.Tasks> Tasks { get; set; }
        public DbSet<TaskFlow.Models.KanbanStage> KanbanStages { get; set; }
    }
}

