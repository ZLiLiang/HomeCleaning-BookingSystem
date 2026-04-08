using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using CY.HomeCleaning.Business;

namespace CY.HomeCleaning.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class HomeCleaningDbContext :
    AbpDbContext<HomeCleaningDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    public DbSet<ServiceItem> ServiceItems { get; set; }
    public DbSet<CapacitySchedule> CapacitySchedules { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<CouponTemplate> CouponTemplates { get; set; }
    public DbSet<UserCoupon> UserCoupons { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public HomeCleaningDbContext(DbContextOptions<HomeCleaningDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        builder.Entity<ServiceItem>(b =>
        {
            b.ToTable(HomeCleaningConsts.DbTablePrefix + "ServiceItems", HomeCleaningConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(HomeCleaningEntityConsts.NameMaxLength);
            b.Property(x => x.BasePrice).HasPrecision(18, 2);
            b.Property(x => x.BillingUnitType).IsRequired();
            b.Property(x => x.IntroductionResourceUrl).HasMaxLength(HomeCleaningEntityConsts.UrlMaxLength);
        });

        builder.Entity<CapacitySchedule>(b =>
        {
            b.ToTable(HomeCleaningConsts.DbTablePrefix + "CapacitySchedules", HomeCleaningConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ServiceDate).HasColumnType("date");
            b.Property(x => x.TimeSlot).IsRequired().HasMaxLength(HomeCleaningEntityConsts.TimeSlotMaxLength);
            b.Property(x => x.MaxCapacity).IsRequired();
            b.Property(x => x.UsedCapacity).IsRequired();
            b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
            b.HasIndex(x => new { x.ServiceDate, x.TimeSlot }).IsUnique();
        });

        builder.Entity<Order>(b =>
        {
            b.ToTable(HomeCleaningConsts.DbTablePrefix + "Orders", HomeCleaningConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.OrderNo).IsRequired().HasMaxLength(HomeCleaningEntityConsts.OrderNoMaxLength);
            b.Property(x => x.TimeSlot).IsRequired().HasMaxLength(HomeCleaningEntityConsts.TimeSlotMaxLength);
            b.Property(x => x.OriginalAmount).HasPrecision(18, 2);
            b.Property(x => x.PaidAmount).HasPrecision(18, 2);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.SnapshotData).IsRequired().HasMaxLength(HomeCleaningEntityConsts.SnapshotDataMaxLength);

            b.HasIndex(x => x.OrderNo).IsUnique();
            b.HasIndex(x => x.CustomerUserId);
            b.HasIndex(x => x.ServiceItemId);
        });

        builder.Entity<CouponTemplate>(b =>
        {
            b.ToTable(HomeCleaningConsts.DbTablePrefix + "CouponTemplates", HomeCleaningConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(HomeCleaningEntityConsts.NameMaxLength);
            b.Property(x => x.FaceValue).HasPrecision(18, 2);
            b.Property(x => x.MinimumSpend).HasPrecision(18, 2);
            b.Property(x => x.TotalCount).IsRequired();
        });

        builder.Entity<UserCoupon>(b =>
        {
            b.ToTable(HomeCleaningConsts.DbTablePrefix + "UserCoupons", HomeCleaningConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Status).IsRequired();
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.CouponTemplateId);
            b.HasIndex(x => x.LockedOrderId);
        });
    }
}
