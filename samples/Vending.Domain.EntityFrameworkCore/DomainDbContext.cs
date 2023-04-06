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
                                       builder.Ignore(s => s.CreatedAt);
                                       builder.Ignore(s => s.CreatedBy);
                                       builder.Ignore(s => s.LastModifiedAt);
                                       builder.Ignore(s => s.LastModifiedBy);
                                       builder.Ignore(s => s.DeletedAt);
                                       builder.Ignore(s => s.DeletedBy);
                                       builder.Ignore(s => s.PictureUrl);
                                       builder.Property(s => s.Name).HasMaxLength(256);
                                       builder.HasIndex(s => new { s.IsDeleted, s.Name });
                                   });
        // Configures the SnackMachine entity
        modelBuilder.Entity<SnackMachine>(builder =>
                                          {
                                              builder.ToTable("SnackMachines");
                                              builder.HasKey(sm => sm.Id);
                                              builder.Ignore(sm => sm.CreatedAt);
                                              builder.Ignore(sm => sm.CreatedBy);
                                              builder.Ignore(sm => sm.LastModifiedAt);
                                              builder.Ignore(sm => sm.LastModifiedBy);
                                              builder.Ignore(sm => sm.DeletedAt);
                                              builder.Ignore(sm => sm.DeletedBy);
                                              builder.OwnsOne(sm => sm.MoneyInside);
                                              builder.Property(sm => sm.AmountInTransaction).HasPrecision(10, 2);
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
                                                                        });
                                  });
        // Configures the Purchase entity
        modelBuilder.Entity<Purchase>(builder =>
                                      {
                                          builder.ToTable("Purchases");
                                          builder.HasKey(p => p.Id);
                                          builder.HasOne<SnackMachine>().WithMany().HasForeignKey(p => p.MachineId).OnDelete(DeleteBehavior.Cascade);
                                          builder.HasOne<Snack>().WithMany().HasForeignKey(p => p.SnackId).OnDelete(DeleteBehavior.Cascade);
                                          builder.Property(p => p.BoughtPrice).HasPrecision(10, 2);
                                          builder.Property(p => p.BoughtBy).HasMaxLength(256);
                                      });
        base.OnModelCreating(modelBuilder);
    }
}
