using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OzoneMobileService.Application.Interfaces;
using OzoneMobileService.Domain.Common;
using OzoneMobileService.Domain.Entities;

namespace OzoneMobileService.Infrastructure.Persistence;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    ITenantContext tenantContext)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    private Guid? CurrentTenantId => tenantContext.TenantId;

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();
    public DbSet<AppNotification> AppNotifications => Set<AppNotification>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<TaxConfiguration> TaxConfigurations => Set<TaxConfiguration>();
    public DbSet<SubscriptionUpgradeRequest> SubscriptionUpgradeRequests => Set<SubscriptionUpgradeRequest>();
    public DbSet<MobileBrand> MobileBrands => Set<MobileBrand>();
    public DbSet<MobileModel> MobileModels => Set<MobileModel>();
    public DbSet<MobileVariant> MobileVariants => Set<MobileVariant>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerDevice> CustomerDevices => Set<CustomerDevice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.Property(rt => rt.Token).HasMaxLength(512).IsRequired();
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("subscription_plans");
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Code).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<AppNotification>(entity =>
        {
            entity.ToTable("app_notifications");
            entity.HasKey(n => n.Id);
            entity.Property(n => n.RoleTarget).HasMaxLength(50).IsRequired();
            entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
            entity.Property(n => n.Message).HasMaxLength(2000).IsRequired();
            entity.Property(n => n.NotificationType).HasMaxLength(50).IsRequired();
            entity.HasIndex(n => new { n.TenantId, n.RoleTarget, n.IsRead });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("invoices");
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => new { i.TenantId, i.InvoiceNumber }).IsUnique();
            entity.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
            entity.Property(i => i.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(i => i.CustomerPhone).HasMaxLength(20).IsRequired();
            entity.Property(i => i.Status).HasMaxLength(20).IsRequired();
            entity.Property(i => i.SubTotal).HasPrecision(18, 2);
            entity.Property(i => i.CgstAmount).HasPrecision(18, 2);
            entity.Property(i => i.SgstAmount).HasPrecision(18, 2);
            entity.Property(i => i.TaxAmount).HasPrecision(18, 2);
            entity.Property(i => i.TotalAmount).HasPrecision(18, 2);
            entity.Property(i => i.InvoiceType).HasMaxLength(20).IsRequired();
            entity.HasOne(i => i.Branch)
                .WithMany()
                .HasForeignKey(i => i.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.Code).IsUnique();
            entity.Property(t => t.Name).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Code).HasMaxLength(50).IsRequired();
            entity.HasOne(t => t.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(t => t.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("branches");
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => new { b.TenantId, b.Code }).IsUnique();
            entity.Property(b => b.Name).HasMaxLength(200).IsRequired();
            entity.Property(b => b.Code).HasMaxLength(50).IsRequired();
            entity.HasOne(b => b.Tenant)
                .WithMany(t => t.Branches)
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaxConfiguration>(entity =>
        {
            entity.ToTable("tax_configurations");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
            entity.Property(t => t.CgstRate).HasPrecision(5, 2);
            entity.Property(t => t.SgstRate).HasPrecision(5, 2);
        });

        modelBuilder.Entity<SubscriptionUpgradeRequest>(entity =>
        {
            entity.ToTable("subscription_upgrade_requests");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Status).HasMaxLength(20).IsRequired();
            entity.Property(r => r.RejectionReason).HasMaxLength(500);
            entity.HasIndex(r => new { r.TenantId, r.Status });
            entity.HasOne(r => r.RequestedPlan)
                .WithMany()
                .HasForeignKey(r => r.RequestedPlanId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.CurrentPlan)
                .WithMany()
                .HasForeignKey(r => r.CurrentPlanId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.Tenant)
                .WithMany()
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.Invoice)
                .WithMany()
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserBranch>(entity =>
        {
            entity.ToTable("user_branches");
            entity.HasKey(ub => new { ub.UserId, ub.BranchId });
            entity.HasOne(ub => ub.User)
                .WithMany()
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ub => ub.Branch)
                .WithMany()
                .HasForeignKey(ub => ub.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MobileBrand>(entity =>
        {
            entity.ToTable("mobile_brands");
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => b.Name).IsUnique();
            entity.Property(b => b.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<MobileModel>(entity =>
        {
            entity.ToTable("mobile_models");
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => new { m.BrandId, m.Name }).IsUnique();
            entity.Property(m => m.Name).HasMaxLength(100).IsRequired();
            entity.HasOne(m => m.Brand)
                .WithMany(b => b.Models)
                .HasForeignKey(m => m.BrandId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MobileVariant>(entity =>
        {
            entity.ToTable("mobile_variants");
            entity.HasKey(v => v.Id);
            entity.HasIndex(v => new { v.ModelId, v.Name }).IsUnique();
            entity.Property(v => v.Name).HasMaxLength(100).IsRequired();
            entity.HasOne(v => v.Model)
                .WithMany(m => m.Variants)
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => new { c.TenantId, c.MobileNumber }).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.MobileNumber).HasMaxLength(10).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(256);
            entity.Property(c => c.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<CustomerDevice>(entity =>
        {
            entity.ToTable("customer_devices");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Imei).HasMaxLength(20);
            entity.HasOne(d => d.Customer)
                .WithMany(c => c.Devices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Variant)
                .WithMany()
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(d => d.RegisteredAtBranch)
                .WithMany()
                .HasForeignKey(d => d.RegisteredAtBranchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureTenantQueryFilters(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private void ConfigureTenantQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            modelBuilder.Entity(entityType.ClrType)
                .HasIndex(nameof(ITenantEntity.TenantId));

            var method = typeof(AppDbContext)
                .GetMethod(nameof(SetTenantQueryFilter), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(this, [modelBuilder]);
        }
    }

    private void SetTenantQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ITenantEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity => CurrentTenantId == null || entity.TenantId == CurrentTenantId);
    }
}
