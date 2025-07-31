using Microsoft.EntityFrameworkCore;
using TM.DAL.Entities.AppEntities;
using TM.DAL.Abstract;

namespace TM.DAL
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<TaskVersion> Versions { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskVersion>()
                .HasOne(tv => tv.Task)
                .WithMany(ut => ut.Versions)
                .HasForeignKey(tv => tv.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.CurrentVersion)
                .WithOne()
                .HasForeignKey<UserTask>(ut => ut.CurrentVersionId)
                .IsRequired(false);

            modelBuilder.Entity<Document>()
               .HasOne(d => d.Task)
               .WithMany(ut => ut.Documents)
               .HasForeignKey(d => d.TaskId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTask>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ut => ut.FirstUpdater)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTask>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ut => ut.LastUpdater)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskVersion>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(tv => tv.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}