using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.Abstractions.SnackMachines;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.Projection.EntityFrameworkCore;

[PublicAPI]
public class ProjectionDbContext : DbContext
{
    public ProjectionDbContext(DbContextOptions<ProjectionDbContext> options)
        : base(options)
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
                                       // builder.Property(s => s.Version).IsConcurrencyToken();
                                       builder.Property(s => s.CreatedBy).HasMaxLength(256);
                                       builder.Property(s => s.LastModifiedBy).HasMaxLength(256);
                                       builder.Property(s => s.DeletedBy).HasMaxLength(256);
                                       builder.Property(s => s.Name).HasMaxLength(256);
                                       builder.Property(s => s.PictureUrl).HasMaxLength(512);
                                       builder.Property(s => s.BoughtAmount).HasPrecision(10, 2);
                                   });
        // Configures the SnackMachine entity
        modelBuilder.Entity<SnackMachine>(builder =>
                                          {
                                              builder.ToTable("SnackMachines");
                                              builder.HasKey(sm => sm.Id);
                                              // builder.Property(sm => sm.Version).IsConcurrencyToken();
                                              builder.Property(sm => sm.CreatedBy).HasMaxLength(256);
                                              builder.Property(sm => sm.LastModifiedBy).HasMaxLength(256);
                                              builder.Property(sm => sm.DeletedBy).HasMaxLength(256);
                                              builder.OwnsOne(sm => sm.MoneyInside, navigation =>
                                                                                    {
                                                                                        navigation.Property(m => m.Amount).HasPrecision(10, 2);
                                                                                    });
                                              builder.Property(sm => sm.AmountInTransaction).HasPrecision(10, 2);
                                              builder.Property(sm => sm.SnackAmount).HasPrecision(10, 2);
                                              builder.Property(sm => sm.BoughtAmount).HasPrecision(10, 2);
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
                                                                            navigationBuilder.Property(sp => sp.SnackName).HasMaxLength(256);
                                                                            navigationBuilder.Property(sp => sp.SnackPictureUrl).HasMaxLength(512);
                                                                            navigationBuilder.Property(sp => sp.Price).HasPrecision(10, 2);
                                                                            navigationBuilder.Property(sp => sp.TotalAmount).HasPrecision(10, 2);
                                                                        });
                                  });
        // Configures the Purchase entity
        modelBuilder.Entity<Purchase>(builder =>
                                      {
                                          builder.ToTable("Purchases");
                                          builder.HasKey(p => p.Id);
                                          builder.HasOne<SnackMachine>().WithMany().HasForeignKey(p => p.MachineId).OnDelete(DeleteBehavior.Cascade);
                                          builder.HasOne<Snack>().WithMany().HasForeignKey(p => p.SnackId).OnDelete(DeleteBehavior.Cascade);
                                          builder.Property(p => p.SnackName).HasMaxLength(256);
                                          builder.Property(p => p.SnackPictureUrl).HasMaxLength(512);
                                          builder.Property(p => p.BoughtPrice).HasPrecision(10, 2);
                                          builder.Property(p => p.BoughtBy).HasMaxLength(256);
                                      });
        base.OnModelCreating(modelBuilder);
    }
}
