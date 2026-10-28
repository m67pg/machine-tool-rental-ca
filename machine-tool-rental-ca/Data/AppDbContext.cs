using machine_tool_rental_ca.Models;
using Microsoft.EntityFrameworkCore;

namespace machine_tool_rental_ca.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<RentalMachine> RentalMachines => Set<RentalMachine>();
    public DbSet<MachineSpec> MachineSpecs => Set<MachineSpec>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RentalMachine>(e =>
        {
            e.Property(x => x.MonthlyPrice).HasPrecision(12, 0);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Category);
            e.HasMany(x => x.Specs)
                .WithOne(s => s.RentalMachine)
                .HasForeignKey(s => s.RentalMachineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MachineSpec>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100);
            e.Property(x => x.Value).HasMaxLength(200);
        });

        modelBuilder.Entity<AdminUser>(e =>
        {
            e.Property(x => x.UserName).HasMaxLength(100);
            e.HasIndex(x => x.UserName).IsUnique();
        });
    }
}
