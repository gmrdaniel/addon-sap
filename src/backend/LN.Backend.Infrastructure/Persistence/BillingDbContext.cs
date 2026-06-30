using LN.Backend.Domain.Profiles;
using LN.Backend.Domain.Usage;
using Microsoft.EntityFrameworkCore;

namespace LN.Backend.Infrastructure.Persistence;

/// <summary>
/// Contexto de persistencia para licencias y eventos de consumo.
/// El provider concreto (PostgreSQL o SQL Server) se configura en el Api
/// una vez confirmada la decisión de Fase 0.
/// </summary>
public sealed class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<UsageEvent> UsageEvents => Set<UsageEvent>();
    public DbSet<License> Licenses => Set<License>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsageEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserKey).HasMaxLength(50);
            e.Property(x => x.CompanyDb).HasMaxLength(50);
        });

        modelBuilder.Entity<License>(e =>
        {
            e.HasKey(x => new { x.UserKey, x.CompanyDb });
            e.Property(x => x.UserKey).HasMaxLength(50);
            e.Property(x => x.CompanyDb).HasMaxLength(50);
        });
    }
}
