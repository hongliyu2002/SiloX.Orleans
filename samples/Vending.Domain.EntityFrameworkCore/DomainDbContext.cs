using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.EntityFrameworkCore;

[PublicAPI]
public class DomainDbContext : DbContext
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
    {
    }

    public DbSet<Snack> Snacks { get; set; } = null!;

    public DbSet<SnackMachine> SnackMachines { get; set; } = null!;

    public DbSet<Purchase> Purchases { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the Snack entity
        modelBuilder.Entity<Snack>(builder =>
                                   {
                                       builder.ToTable("Snacks");
                                       builder.HasKey(s => s.Id);
                                       builder.Property(s => s.CreatedBy).HasMaxLength(256);
                                       builder.Property(s => s.LastModifiedBy).HasMaxLength(256);
                                       builder.Property(s => s.DeletedBy).HasMaxLength(256);
                                       builder.Property(s => s.Name).HasMaxLength(256);
                                       builder.Property(s => s.PictureUrl).HasMaxLength(512);
                                   });
        // Configures the SnackMachine entity
        modelBuilder.Entity<SnackMachine>(builder =>
                                          {
                                              builder.ToTable("SnackMachines");
                                              builder.HasKey(sm => sm.Id);
                                              builder.Property(sm => sm.CreatedBy).HasMaxLength(256);
                                              builder.Property(sm => sm.LastModifiedBy).HasMaxLength(256);
                                              builder.Property(sm => sm.DeletedBy).HasMaxLength(256);
                                              builder.OwnsOne(sm => sm.MoneyInside);
                                              builder.Property(sm => sm.AmountInTransaction).HasPrecision(10, 2);
                                          });
        // Configures the Slot entity
        modelBuilder.Entity<Slot>(builder =>
                                  {
                                      builder.ToTable("Slots");
                                      builder.HasKey(s => new { s.MachineId, s.Position });
                                      builder.HasOne<SnackMachine>().WithMany(m => m.Slots).HasForeignKey(s => s.MachineId).OnDelete(DeleteBehavior.Cascade);
                                      builder.OwnsOne(s => s.SnackPile, navigationBuilder =>
                                                                        {
                                                                            navigationBuilder.HasOne<Snack>().WithMany().HasForeignKey(sp => sp.SnackId).OnDelete(DeleteBehavior.Cascade);
                                                                            navigationBuilder.Property(sp => sp.Price).HasPrecision(10, 2);
                                                                        });
                                  });
        // Configures the Purchase entity
        modelBuilder.Entity<Purchase>(builder =>
                                      {
                                          builder.ToTable("Purchases");
                                          builder.HasKey(sb => new
                                                               {
                                                                   sb.MachineId,
                                                                   sb.Position,
                                                                   sb.SnackId
                                                               });
                                          builder.HasOne<SnackMachine>().WithMany().HasForeignKey(p => p.MachineId).OnDelete(DeleteBehavior.Cascade);
                                          builder.HasOne<Snack>().WithMany().HasForeignKey(p => p.SnackId).OnDelete(DeleteBehavior.Cascade);
                                          builder.Property(sb => sb.BoughtPrice).HasPrecision(10, 2);
                                          builder.Property(sb => sb.BoughtBy).HasMaxLength(256);
                                      });
        base.OnModelCreating(modelBuilder);
    }
}
