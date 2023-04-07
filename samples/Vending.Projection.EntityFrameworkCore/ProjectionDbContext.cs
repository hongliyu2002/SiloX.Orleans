using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.Abstractions.Snacks;

namespace Vending.Projection.EntityFrameworkCore;

[PublicAPI]
public class ProjectionDbContext : DbContext
{
    public ProjectionDbContext(DbContextOptions<ProjectionDbContext> options)
        : base(options)
    {
    }

    public DbSet<SnackInfo> Snacks { get; set; } = null!;

    public DbSet<MachineInfo> Machines { get; set; } = null!;

    public DbSet<PurchaseInfo> Purchases { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the SnackInfo entity
        modelBuilder.Entity<SnackInfo>(builder =>
                                       {
                                           builder.ToTable("Snacks");
                                           builder.HasKey(s => s.Id);
                                           builder.Property(s => s.Name).HasMaxLength(128);
                                           builder.Property(s => s.PictureUrl).HasMaxLength(512);
                                           builder.Property(s => s.CreatedBy).HasMaxLength(128);
                                           builder.Property(s => s.LastModifiedBy).HasMaxLength(128);
                                           builder.Property(s => s.DeletedBy).HasMaxLength(128);
                                           builder.Property(m => m.TotalAmount).HasPrecision(10, 2);
                                           builder.Property(s => s.BoughtAmount).HasPrecision(10, 2);
                                           builder.HasIndex(s => new { s.IsDeleted, s.CreatedAt });
                                           builder.HasIndex(s => new { s.IsDeleted, s.LastModifiedAt });
                                           builder.HasIndex(s => new { s.IsDeleted, s.Name });
                                       });
        // Configures the MachineInfo entity
        modelBuilder.Entity<MachineInfo>(builder =>
                                         {
                                             builder.ToTable("Machines");
                                             builder.HasKey(m => m.Id);
                                             builder.OwnsOne(m => m.MoneyInside, navigation =>
                                                                                     {
                                                                                         navigation.Property(m => m.Amount).HasPrecision(10, 2);
                                                                                     });
                                             builder.Property(m => m.AmountInTransaction).HasPrecision(10, 2);
                                             builder.Property(m => m.CreatedBy).HasMaxLength(128);
                                             builder.Property(m => m.LastModifiedBy).HasMaxLength(128);
                                             builder.Property(m => m.DeletedBy).HasMaxLength(128);
                                             builder.Property(m => m.SnackAmount).HasPrecision(10, 2);
                                             builder.Property(m => m.BoughtAmount).HasPrecision(10, 2);
                                             builder.HasIndex(m => new { m.IsDeleted, m.CreatedAt });
                                             builder.HasIndex(m => new { m.IsDeleted, m.LastModifiedAt });
                                         });
        // Configures the MachineSlotInfo entity
        modelBuilder.Entity<MachineSlotInfo>(builder =>
                                             {
                                                 builder.ToTable("MachineSlots");
                                                 builder.HasKey(ms => new { ms.MachineId, ms.Position });
                                                 builder.HasOne<MachineInfo>().WithMany(m => m.Slots).HasForeignKey(ms => ms.MachineId).OnDelete(DeleteBehavior.Cascade);
                                                 builder.OwnsOne(ms => ms.SnackPile, navigationBuilder =>
                                                                                     {
                                                                                         navigationBuilder.HasOne<SnackInfo>().WithMany().HasForeignKey(sp => sp.SnackId).OnDelete(DeleteBehavior.Cascade);
                                                                                         navigationBuilder.Property(sp => sp.SnackName).HasMaxLength(128);
                                                                                         navigationBuilder.Property(sp => sp.SnackPictureUrl).HasMaxLength(512);
                                                                                         navigationBuilder.Property(sp => sp.Price).HasPrecision(10, 2);
                                                                                         navigationBuilder.Property(sp => sp.Amount).HasPrecision(10, 2);
                                                                                     });
                                             });
        // Configures the PurchaseInfo entity
        modelBuilder.Entity<PurchaseInfo>(builder =>
                                          {
                                              builder.ToTable("Purchases");
                                              builder.HasKey(p => p.Id);
                                              builder.HasOne<MachineInfo>().WithMany().HasForeignKey(p => p.MachineId).OnDelete(DeleteBehavior.Cascade);
                                              builder.HasOne<SnackInfo>().WithMany().HasForeignKey(p => p.SnackId).OnDelete(DeleteBehavior.Cascade);
                                              builder.Property(p => p.SnackName).HasMaxLength(128);
                                              builder.Property(p => p.SnackPictureUrl).HasMaxLength(512);
                                              builder.Property(p => p.BoughtPrice).HasPrecision(10, 2);
                                              builder.Property(p => p.BoughtBy).HasMaxLength(128);
                                              builder.HasIndex(p => p.SnackName);
                                          });
        base.OnModelCreating(modelBuilder);
    }
}
