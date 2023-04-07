using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.Domain.EntityFrameworkCore;

[PublicAPI]
public class DomainDbContext : DbContext
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options)
        : base(options)
    {
    }

    public DbSet<Snack> Snacks { get; set; } = null!;

    public DbSet<Machine> Machines { get; set; } = null!;

    public DbSet<Purchase> Purchases { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the Snack entity
        modelBuilder.Entity<Snack>(builder =>
                                   {
                                       builder.ToTable("Snacks");
                                       builder.HasKey(s => s.Id);
                                       builder.Property(s => s.Name).HasMaxLength(128);
                                       builder.Ignore(s => s.CreatedAt);
                                       builder.Ignore(s => s.CreatedBy);
                                       builder.Ignore(s => s.LastModifiedAt);
                                       builder.Ignore(s => s.LastModifiedBy);
                                       builder.Ignore(s => s.DeletedAt);
                                       builder.Ignore(s => s.DeletedBy);
                                       builder.Ignore(s => s.PictureUrl);
                                       builder.Property(s => s.Name).HasMaxLength(128);
                                       builder.HasIndex(s => new { s.IsDeleted, s.Name });
                                   });
        // Configures the Machine entity
        modelBuilder.Entity<Machine>(builder =>
                                     {
                                         builder.ToTable("Machines");
                                         builder.HasKey(m => m.Id);
                                         builder.OwnsOne(m => m.MoneyInside, navigation =>
                                                                             {
                                                                                 navigation.Property(m => m.Amount).HasPrecision(10, 2);
                                                                             });
                                         builder.Property(m => m.AmountInTransaction).HasPrecision(10, 2);
                                         builder.Property(m => m.SnackAmount).HasPrecision(10, 2);
                                         builder.Ignore(m => m.CreatedAt);
                                         builder.Ignore(m => m.CreatedBy);
                                         builder.Ignore(m => m.LastModifiedAt);
                                         builder.Ignore(m => m.LastModifiedBy);
                                         builder.Ignore(m => m.DeletedAt);
                                         builder.Ignore(m => m.DeletedBy);
                                         builder.HasIndex(s => new { s.IsDeleted });
                                     });
        // Configures the Slot entity
        modelBuilder.Entity<MachineSlot>(builder =>
                                         {
                                             builder.ToTable("MachineSlots");
                                             builder.HasKey(ms => new { ms.MachineId, ms.Position });
                                             builder.HasOne<Machine>().WithMany(m => m.Slots).HasForeignKey(ms => ms.MachineId).OnDelete(DeleteBehavior.Cascade);
                                             builder.OwnsOne(ms => ms.SnackPile, navigationBuilder =>
                                                                                 {
                                                                                     navigationBuilder.HasOne<Snack>().WithMany().HasForeignKey(sp => sp.SnackId).OnDelete(DeleteBehavior.Cascade);
                                                                                     navigationBuilder.Property(sp => sp.Price).HasPrecision(10, 2);
                                                                                     navigationBuilder.Property(sp => sp.Amount).HasPrecision(10, 2);
                                                                                 });
                                         });
        // Configures the MachineSnackStat entity
        modelBuilder.Entity<MachineSnackStat>(builder =>
                                              {
                                                  builder.ToTable("MachineSnackStats");
                                                  builder.HasKey(ss => new { ss.MachineId, ss.SnackId });
                                                  builder.HasOne<Machine>().WithMany(m => m.SnackStats).HasForeignKey(ss => ss.MachineId).OnDelete(DeleteBehavior.Cascade);
                                                  builder.HasOne<Snack>().WithMany().HasForeignKey(ss => ss.SnackId).OnDelete(DeleteBehavior.Cascade);
                                                  builder.Property(ss => ss.TotalAmount).HasPrecision(10, 2);
                                              });
        // Configures the Purchase entity
        modelBuilder.Entity<Purchase>(builder =>
                                      {
                                          builder.ToTable("Purchases");
                                          builder.HasKey(p => p.Id);
                                          builder.HasOne<Machine>().WithMany().HasForeignKey(p => p.MachineId).OnDelete(DeleteBehavior.Cascade);
                                          builder.HasOne<Snack>().WithMany().HasForeignKey(p => p.SnackId).OnDelete(DeleteBehavior.Cascade);
                                          builder.Property(p => p.BoughtPrice).HasPrecision(10, 2);
                                          builder.Property(p => p.BoughtBy).HasMaxLength(128);
                                      });
        base.OnModelCreating(modelBuilder);
    }
}
