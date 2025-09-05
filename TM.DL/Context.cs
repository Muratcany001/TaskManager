using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<TaskVersion> Versions { get; set; }
        public DbSet<Document> Documents { get; set; }
        //public DbSet<Roles> Roles { get; set; }
        //public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Roles many-to-many
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRoles"));

            // TaskVersion -> UserTask (Versions)
            modelBuilder.Entity<TaskVersion>()
                .HasOne(tv => tv.Task)
                .WithMany(ut => ut.Versions)
                .HasForeignKey(tv => tv.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserTask -> CurrentVersion (optional one-to-one)
            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.CurrentVersion)
                .WithOne()
                .HasForeignKey<UserTask>(ut => ut.CurrentVersionId)
                .IsRequired(false);

            // Document -> UserTask
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Task)
                .WithMany(ut => ut.Documents)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserTask -> User (FirstUpdater / LastUpdater)
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

            // TaskVersion -> User (CreatedByUserId)
            modelBuilder.Entity<TaskVersion>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(tv => tv.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Token -> User
            modelBuilder.Entity<Token>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    // Design-time DbContext Factory
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-KP4MLAD\\MURAT;Database=FileManager;TrustServerCertificate=True;Trusted_Connection=True;");

            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(
                    RelationalEventId.PendingModelChangesWarning,
                    RelationalEventId.MultipleCollectionIncludeWarning
                ));

            return new Context(optionsBuilder.Options);
        }
    }
}
