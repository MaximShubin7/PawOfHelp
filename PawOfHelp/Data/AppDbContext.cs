// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Models;

namespace PawOfHelp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<OrganizationDetails> OrganizationDetails { get; set; }
    public DbSet<Competency> Competencies { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Preference> Preferences { get; set; }
    public DbSet<ConstantNeed> ConstantNeeds { get; set; }
    public DbSet<Theme> Themes { get; set; }
    public DbSet<AnimalType> AnimalTypes { get; set; }
    public DbSet<Animal> Animals { get; set; }
    public DbSet<HelpTask> HelpTasks { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostPhoto> PostPhotos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<UserCompetency> UserCompetencies { get; set; }
    public DbSet<UserAvailability> UserAvailabilities { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<OrganizationConstantNeed> OrganizationConstantNeeds { get; set; }
    public DbSet<UserAnimal> UserAnimals { get; set; }
    public DbSet<TaskAnimal> TaskAnimals { get; set; }
    public DbSet<TaskCompetency> TaskCompetencies { get; set; }
    public DbSet<TaskLocation> TaskLocations { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<ReferenceBook> ReferenceBook { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);

            entity.HasOne(e => e.Role)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId);

            entity.HasOne(e => e.Location)
                  .WithMany(l => l.Users)
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.OrganizationDetails)
                  .WithOne(o => o.User)
                  .HasForeignKey<OrganizationDetails>(o => o.UserId);
        });

        modelBuilder.Entity<OrganizationDetails>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Website).HasMaxLength(255);
            entity.Property(e => e.DonationDetails).HasMaxLength(500);
        });

        modelBuilder.Entity<Competency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasMany(e => e.UserCompetencies)
                  .WithOne(uc => uc.Competency)
                  .HasForeignKey(uc => uc.CompetencyId);
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<ConstantNeed>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<AnimalType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Breed).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Health).HasMaxLength(500);
            entity.Property(e => e.Character).HasMaxLength(500);
            entity.Property(e => e.SpecialNeeds).HasMaxLength(1000);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);

            entity.HasOne(e => e.AnimalType)
                  .WithMany()
                  .HasForeignKey(e => e.AnimalTypeId);
        });

        modelBuilder.Entity<HelpTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.IsTaskOverexposure).HasDefaultValue(false);
            entity.HasIndex(e => e.CreatorId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.StartedAt);
            entity.HasIndex(e => e.EndedAt);
            entity.HasIndex(e => e.IsTaskOverexposure);

            entity.HasOne(e => e.Creator)
                  .WithMany()
                  .HasForeignKey(e => e.CreatorId);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostPhoto>(entity =>
        {
            entity.HasKey(e => new { e.PostId, e.PhotoUrl });
            entity.Property(e => e.PhotoUrl).HasMaxLength(500);
            entity.HasIndex(e => e.PostId);

            entity.HasOne(e => e.Post)
                  .WithMany(p => p.Photos)
                  .HasForeignKey(e => e.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.RecipientId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.SenderId);

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.SenderId);

            entity.HasOne(e => e.Recipient)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientId);
        });

        modelBuilder.Entity<VerificationCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(6);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => e.TaskId);

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.SenderId);

            entity.HasOne(e => e.HelpTask)
                  .WithMany()
                  .HasForeignKey(e => e.TaskId);

            entity.HasOne(e => e.Status)
                  .WithMany()
                  .HasForeignKey(e => e.StatusId);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<UserCompetency>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CompetencyId });

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Competency)
                  .WithMany(c => c.UserCompetencies)
                  .HasForeignKey(e => e.CompetencyId);
        });

        modelBuilder.Entity<UserAvailability>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.AvailabilityId });

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Availability)
                  .WithMany()
                  .HasForeignKey(e => e.AvailabilityId);
        });

        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PreferenceId });

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Preference)
                  .WithMany()
                  .HasForeignKey(e => e.PreferenceId);
        });

        modelBuilder.Entity<OrganizationConstantNeed>(entity =>
        {
            entity.HasKey(e => new { e.OrganizationId, e.ConstantNeedId });

            entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrganizationId);

            entity.HasOne(e => e.ConstantNeed)
                  .WithMany()
                  .HasForeignKey(e => e.ConstantNeedId);
        });

        modelBuilder.Entity<UserAnimal>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.AnimalId });

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.AnimalId);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Animal)
                  .WithMany()
                  .HasForeignKey(e => e.AnimalId);
        });

        modelBuilder.Entity<TaskAnimal>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.AnimalId });

            entity.HasIndex(e => e.TaskId);
            entity.HasIndex(e => e.AnimalId);

            entity.HasOne(e => e.HelpTask)
                  .WithMany()
                  .HasForeignKey(e => e.TaskId);

            entity.HasOne(e => e.Animal)
                  .WithMany()
                  .HasForeignKey(e => e.AnimalId);
        });

        modelBuilder.Entity<TaskCompetency>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.CompetencyId });

            entity.HasOne(e => e.HelpTask)
                  .WithMany()
                  .HasForeignKey(e => e.TaskId);

            entity.HasOne(e => e.Competency)
                  .WithMany()
                  .HasForeignKey(e => e.CompetencyId);
        });

        modelBuilder.Entity<TaskLocation>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.LocationId });

            entity.HasOne(e => e.HelpTask)
                  .WithMany()
                  .HasForeignKey(e => e.TaskId);

            entity.HasOne(e => e.Location)
                  .WithMany()
                  .HasForeignKey(e => e.LocationId);
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TaskId });

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TaskId);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.HelpTask)
                  .WithMany()
                  .HasForeignKey(e => e.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReferenceBook>(entity =>
        {
            entity.HasKey(e => new { e.AnimalTypeId, e.ThemeId });

            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);

            entity.HasOne(e => e.AnimalType)
                  .WithMany()
                  .HasForeignKey(e => e.AnimalTypeId);

            entity.HasOne(e => e.Theme)
                  .WithMany()
                  .HasForeignKey(e => e.ThemeId);
        });
    }
}