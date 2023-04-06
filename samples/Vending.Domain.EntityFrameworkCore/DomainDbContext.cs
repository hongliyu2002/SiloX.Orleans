using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.SnackMachines;
using Vending.Domain.Abstractions.Snacks;

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
                                       builder.Property(s => s.CreatedBy).HasMaxLength(128);
                                       builder.Property(s => s.LastModifiedBy).HasMaxLength(128);
                                       builder.Property(s => s.DeletedBy).HasMaxLength(128);
                                       builder.Property(s => s.Name).HasMaxLength(128);
                                       builder.Property(s => s.PictureUrl).HasMaxLength(512);
                                       builder.HasIndex(s => new { s.IsDeleted, s.Name });
                                   });
        // Configures the SnackMachine entity
        modelBuilder.Entity<SnackMachine>(builder =>
                                          {
                                              builder.ToTable("SnackMachines");
                                              builder.HasKey(sm => sm.Id);
                                              builder.Property(sm => sm.CreatedBy).HasMaxLength(128);
                                              builder.Property(sm => sm.LastModifiedBy).HasMaxLength(128);
                                              builder.Property(sm => sm.DeletedBy).HasMaxLength(128);
                                              builder.OwnsOne(sm => sm.MoneyInside, navigation =>
                                                                                    {
                                                                                        navigation.Property(m => m.Amount).HasPrecision(10, 2);
                                                                                    });
                                              builder.Property(sm => sm.AmountInTransaction).HasPrecision(10, 2);
                                              builder.Property(sm => sm.SnackAmount).HasPrecision(10, 2);
                                          });
        // Configures the Slot entity
        modelBuilder.Entity<Slot>(builder =>
                                  {
                                      builder.ToTable("Slots");
                                      builder.HasKey(sl => new { sl.MachineId, sl.Position });
                                      builder.HasOne<SnackMachine>().WithMany(sm => sm.Slots).HasForeignKey(sl => sl.MachineId).OnDelete(DeleteBehavior.Cascade);
                                      builder.OwnsOne(sl => sl.SnackPile, navigationBuilder =>
                                                                        {
                                                                            navigationBuilder.HasOne<Snack>().WithMany().HasForeignKey(sp => sp.SnackId).OnDelete(DeleteBehavior.Cascade);
                                                                            navigationBuilder.Property(sp => sp.Price).HasPrecision(10, 2);
                                                                            navigationBuilder.Property(sp => sp.Amount).HasPrecision(10, 2);
                                                                        });
                                  });
        // Configures the SnackStat entity
        modelBuilder.Entity<SnackStat>(builder =>
                                        {
                                            builder.HasKey(ss => new { ss.MachineId, ss.SnackId });
                                            builder.HasOne<SnackMachine>().WithMany(sm => sm.SnackStats).HasForeignKey(ss => ss.MachineId).OnDelete(DeleteBehavior.Cascade);
                                            builder.HasOne<Snack>().WithMany().HasForeignKey(ss => ss.SnackId).OnDelete(DeleteBehavior.Cascade);
                                            builder.Property(ss => ss.TotalAmount).HasPrecision(10, 2);
                                        });
        // Configures the Purchase entity
        modelBuilder.Entity<Purchase>(builder =>
                                      {
                                          builder.ToTable("Purchases");
                                          builder.HasKey(p => p.Id);
                                          builder.HasOne<SnackMachine>().WithMany().HasForeignKey(p => p.MachineId).OnDelete(DeleteBehavior.Cascade);
                                          builder.HasOne<Snack>().WithMany().HasForeignKey(p => p.SnackId).OnDelete(DeleteBehavior.Cascade);
                                          builder.Property(p => p.BoughtPrice).HasPrecision(10, 2);
                                          builder.Property(p => p.BoughtBy).HasMaxLength(128);
                                      });
        base.OnModelCreating(modelBuilder);
    }
}
