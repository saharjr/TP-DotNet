using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Compte> Comptes { get; set; } = null!;
    public DbSet<Operation> Operations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(e =>
        {
            e.HasIndex(c => c.Email).IsUnique();
        });

        modelBuilder.Entity<Compte>(e =>
        {
            e.HasIndex(c => c.NumeroCompte).IsUnique();

            e.Property(c => c.TypeCompte)
                .HasConversion<string>()
                .HasMaxLength(20);

            e.HasOne(c => c.Client)
                .WithMany(c => c.Comptes)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Operation>(e =>
        {
            e.Property(o => o.TypeOperation)
                .HasConversion<string>()
                .HasMaxLength(20);

            e.HasOne(o => o.Compte)
                .WithMany(c => c.Operations)
                .HasForeignKey(o => o.CompteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}