using GreenBowl.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;


namespace GreenBowl.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets (tables)
        public DbSet<BatchingControlEquipment> BatchingControlEquipments { get; set; }
        public DbSet<BatchBCE> BatchBCEs { get; set; }
        public DbSet<BulgingTestOnPouch> BulgingTestOnPouches { get; set; }
        public DbSet<BatchingControlBatchingStep> BatchingControlBatchingSteps { get; set; }
        public DbSet<BatchBCBS> BatchBCBS { get; set; }
        public DbSet<BatchingControlPackaging> BatchingControlPackagings { get; set; }
        public DbSet<BatchBCPa> BatchBCPas { get; set; }
        public DbSet<BCPaChecks> BCPaChecks { get; set; }
        public DbSet<BatchingControlProcessing> BatchingControlProcessings { get; set; }
        public DbSet<BatchBCPr> BatchBCPrs { get; set; }
        public DbSet<BCPrChecks> BCPrChecks { get; set; }
        public DbSet<XRayMonitoringRecord> XRayMonitoringRecords { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<IngredientsInventory> IngredientsInventories { get; set; }
        public DbSet<InboundTrailer> InboundTrailers { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var equipmentsConverter = new ValueConverter<List<EquipmentType>, string>(
                v => string.Join(',', v.Select(x => x.ToString())),
                v => string.IsNullOrWhiteSpace(v) ? new List<EquipmentType>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Enum.Parse<EquipmentType>(s))
                .ToList()
            );

            var equipmentsComparer = new ValueComparer<List<EquipmentType>>(
                (a, b) => a.SequenceEqual(b),
                v => v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                v => v.ToList()
            );

            modelBuilder.Entity<BatchBCE>()
                .Property(x => x.Equipments)
                .HasConversion(equipmentsConverter)
                .Metadata.SetValueComparer(equipmentsComparer);

            // Oracle-friendly column type (safe)
            modelBuilder.Entity<BatchBCE>()
                .Property(x => x.Equipments)
                .HasColumnType("VARCHAR2(4000)");



            // Primary Keys
            modelBuilder.Entity<BatchingControlEquipment>().HasKey(x => new { x.Batch, x.Lot });
            modelBuilder.Entity<BatchBCE>().HasKey(x => new { x.Batch, x.Lot });

            modelBuilder.Entity<BatchingControlPackaging>().HasKey(x => new { x.Batch, x.Lot });
            modelBuilder.Entity<BatchBCPa>().HasKey(x => new { x.Batch, x.Lot });
            modelBuilder.Entity<BCPaChecks>().HasKey(x => new { x.Batch, x.Lot, x.TimeOfCheck });

            modelBuilder.Entity<BatchingControlProcessing>().HasKey(x => new { x.Batch, x.Lot });
            modelBuilder.Entity<BatchBCPr>().HasKey(x => new { x.Batch, x.Lot });
            modelBuilder.Entity<BCPrChecks>().HasKey(x => new { x.Batch, x.Lot, x.TimeOfCheck });

            modelBuilder.Entity<BatchingControlBatchingStep>().HasKey(x => new { x.Batch, x.Lot });
            modelBuilder.Entity<BatchBCBS>().HasKey(x => new { x.Batch, x.Lot, x.IngredientLot });

            modelBuilder.Entity<BulgingTestOnPouch>().HasKey(x => x.Lot);
            modelBuilder.Entity<XRayMonitoringRecord>().HasKey(x => new { x.Batch, x.Lot });

            modelBuilder.Entity<ProductInventory>().HasKey(x => x.Lot);
            modelBuilder.Entity<IngredientsInventory>().HasKey(x => x.IngredientLot);
            modelBuilder.Entity<InboundTrailer>().HasKey(x => x.IngredientLot);
            modelBuilder.Entity<Receipt>().HasKey(x => x.ID);

            // Uniqueness: one IngredientName per (Batch, Lot)
            modelBuilder.Entity<BatchBCBS>()
                .HasIndex(x => new { x.Batch, x.Lot, x.IngredientName })
                .IsUnique();

            // 1:1 Header–Detail (shared key)

            // Packaging Header <-> Packaging Detail
            modelBuilder.Entity<BatchBCPa>()
                .HasOne(x => x.Parent)
                .WithOne(x => x.Child)
                .HasForeignKey<BatchBCPa>(x => new { x.Batch, x.Lot })
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 

            // Equipment Header <-> Equipment Detail
            modelBuilder.Entity<BatchBCE>()
                .HasOne(x => x.Parent)
                .WithOne(x => x.Child)
                .HasForeignKey<BatchBCE>(x => new { x.Batch, x.Lot })
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 

            // Processing Header <-> Processing Detail
            modelBuilder.Entity<BatchBCPr>()
                .HasOne(x => x.Parent)
                .WithOne(x => x.Child)
                .HasForeignKey<BatchBCPr>(x => new { x.Batch, x.Lot })
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 

            // IngredientsInventory <-> InboundTrailer
            modelBuilder.Entity<InboundTrailer>()
                .HasOne<IngredientsInventory>()
                .WithOne()
                .HasForeignKey<InboundTrailer>(x => x.IngredientLot)
                .OnDelete(DeleteBehavior.Restrict);

            // 1:N Checks

            // BatchBCPa has ICollection<BCPaChecks> Checks
            modelBuilder.Entity<BCPaChecks>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Child)
                .HasForeignKey(x => new { x.Batch, x.Lot })
                .OnDelete(DeleteBehavior.Cascade);

            // Processing checks 
            modelBuilder.Entity<BCPrChecks>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Child)
                .HasForeignKey(x => new { x.Batch, x.Lot })
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N BatchingStep -> Ingredient rows
            modelBuilder.Entity<BatchBCBS>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Child)
                .HasForeignKey(x => new { x.Batch, x.Lot })
                .OnDelete(DeleteBehavior.Cascade); 

            // IngredientLot FK -> IngredientsInventory for BatchBCBS
            modelBuilder.Entity<BatchBCBS>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientLot)
                .OnDelete(DeleteBehavior.Restrict);

            // Link headers/details to ProductInventory via Lot

            // Headers CASCADE
            modelBuilder.Entity<BatchingControlEquipment>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BatchingControlPackaging>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BatchingControlProcessing>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BatchingControlBatchingStep>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);

            // Details RESTRICT (avoid multiple cascade paths)
            modelBuilder.Entity<BatchBCE>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BatchBCPa>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BatchBCPr>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BatchBCBS>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Restrict);

            // Independent tables
            modelBuilder.Entity<XRayMonitoringRecord>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BulgingTestOnPouch>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Receipt>()
                .HasOne<ProductInventory>()
                .WithMany()
                .HasForeignKey(x => x.Lot)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
