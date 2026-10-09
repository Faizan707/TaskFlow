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
        public DbSet<TaskFlow.Models.Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskFlow.Models.Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(n => n.Actor)
                    .WithMany()
                    .HasForeignKey(n => n.ActorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Task)
                    .WithMany()
                    .HasForeignKey(n => n.TaskId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}


