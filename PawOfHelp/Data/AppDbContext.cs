// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Models;

namespace PawOfHelp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<OrganizationDetails> OrganizationsDetails { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhotoUrl).HasMaxLength(255);
        });

        modelBuilder.Entity<VerificationCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
            entity.Property(e => e.Code).HasMaxLength(6);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<OrganizationDetails>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasOne(e => e.User)
                  .WithOne(u => u.OrganizationDetails)
                  .HasForeignKey<OrganizationDetails>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Rating);
            entity.HasIndex(e => e.RecipientId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Recipient)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}