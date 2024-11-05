﻿using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Addresses;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.HospitalDepartments;
using Pusula.Training.HealthCare.Hospitals;
using Pusula.Training.HealthCare.Notifications;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
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

namespace Pusula.Training.HealthCare.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class HealthCareDbContext :
    AbpDbContext<HealthCareDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Protocol> Protocols { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Hospital> Hospitals { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    //public DbSet<HospitalDepartment> HospitalDepartment { get; set; } = null!; 


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

    public HealthCareDbContext(DbContextOptions<HealthCareDbContext> options)
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
        if (builder.IsHostDatabase())
        {
            #region Patient

            builder.Entity<Patient>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Patients", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasIndex(e => new { e.FirstName, e.LastName, e.IdentityNumber });
                b.Property(x => x.FirstName).HasColumnName(nameof(Patient.FirstName)).IsRequired()
                    .HasMaxLength(PatientConsts.FirstNameMaxLength);
                b.Property(x => x.LastName).HasColumnName(nameof(Patient.LastName)).IsRequired()
                    .HasMaxLength(PatientConsts.LastNameMaxLength);
                b.Property(x => x.BirthDate).HasColumnName(nameof(Patient.BirthDate));
                b.Property(x => x.IdentityNumber).HasColumnName(nameof(Patient.IdentityNumber)).IsRequired()
                    .HasMaxLength(PatientConsts.IdentityNumberMaxLength);
                b.Property(x => x.EmailAddress).HasColumnName(nameof(Patient.EmailAddress)).IsRequired()
                    .HasMaxLength(PatientConsts.EmailAddressMaxLength);
                b.Property(x => x.MobilePhoneNumber).HasColumnName(nameof(Patient.MobilePhoneNumber)).IsRequired()
                    .HasMaxLength(PatientConsts.PhoneNumberMaxLength);
                b.Property(x => x.HomePhoneNumber).HasColumnName(nameof(Patient.HomePhoneNumber)).IsRequired(false)
                    .HasMaxLength(PatientConsts.PhoneNumberMaxLength);
                b.Property(x => x.Gender).HasColumnName(nameof(Patient.Gender)).IsRequired();
                b.Property(x => x.BloodType).HasColumnName(nameof(Patient.BloodType)).IsRequired();
                b.Property(x => x.MaritalStatus).HasColumnName(nameof(Patient.MaritalStatus)).IsRequired();

                b.HasOne<Country>().WithMany().IsRequired().HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region Country

            builder.Entity<Country>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Countries", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Country.Name)).IsRequired()
                    .HasMaxLength(CountryConsts.NameMaxLength);
                b.Property(x => x.Abbreviation).HasColumnName(nameof(Country.Abbreviation)).IsRequired()
                    .HasMaxLength(CountryConsts.AbbreviationMaxLength);
            });

            #endregion

            #region City

            builder.Entity<City>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Cities", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasIndex(e => e.CountryId);

                b.Property(e => e.Name).HasColumnName(nameof(City.Name)).IsRequired()
                    .HasMaxLength(CityConsts.NameMaxLength);

                b.HasOne<Country>().WithMany().IsRequired().HasForeignKey(e => e.CountryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region District

            builder.Entity<District>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Districts", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasIndex(e => e.CityId);

                b.Property(e => e.Name).HasColumnName(nameof(District.Name)).IsRequired()
                    .HasMaxLength(DistrictConsts.NameMaxLength);

                b.HasOne<City>().WithMany().IsRequired().HasForeignKey(e => e.CityId).OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region Address

            builder.Entity<Address>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Addresses", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasIndex(e => e.PatientId);

                b.Property(e => e.AddressLine).HasColumnName(nameof(Address.AddressLine)).IsRequired();

                b.HasOne<District>().WithMany().IsRequired().HasForeignKey(e => e.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Patient>().WithOne().IsRequired().HasForeignKey<Address>(e => e.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            //builder.Entity<Department>(b =>
            //{
            //    b.ToTable(HealthCareConsts.DbTablePrefix + "Departments", HealthCareConsts.DbSchema);
            //    b.ConfigureByConvention();
            //    b.Property(x => x.Name).HasColumnName(nameof(Department.Name)).IsRequired().HasMaxLength(DepartmentConsts.NameMaxLength);
            //});

            #endregion

            builder.Entity<Protocol>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Protocols", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Type).HasColumnName(nameof(Protocol.Type)).IsRequired()
                    .HasMaxLength(ProtocolConsts.TypeMaxLength);
                b.Property(x => x.StartTime).HasColumnName(nameof(Protocol.StartTime));
                b.Property(x => x.EndTime).HasColumnName(nameof(Protocol.EndTime));
                b.HasOne<Patient>().WithMany().IsRequired().HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Department>().WithMany().IsRequired().HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Department>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Departments", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Department.Name)).IsRequired().HasMaxLength(DepartmentConsts.NameMaxLength);   
                b.Property(x => x.Description).HasColumnName(nameof(Department.Description)).HasMaxLength(DepartmentConsts.DescriptionMaxLength);
                b.Property(x => x.Duration).HasColumnName(nameof(Department.Duration)).IsRequired();
                b.HasMany<HospitalDepartment>().WithOne().HasForeignKey(x => x.DepartmentId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Hospital>(Hospitals =>
            {
                Hospitals.ToTable(HealthCareConsts.DbTablePrefix + "Hospitals", HealthCareConsts.DbSchema);
                Hospitals.ConfigureByConvention();
                Hospitals.Property(x => x.Name).HasColumnName(nameof(Hospital.Name)).IsRequired().HasMaxLength(HospitalConsts.NameMaxLength);
                Hospitals.Property(x => x.Address).HasColumnName(nameof(Hospital.Address)).IsRequired().HasMaxLength(HospitalConsts.AddressMaxLength); 
                Hospitals.HasMany<HospitalDepartment>().WithOne().HasForeignKey(x => x.HospitalId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Notification>(Notifications => {
                Notifications.ToTable(HealthCareConsts.DbTablePrefix + "Notifications", HealthCareConsts.DbSchema);
                Notifications.ConfigureByConvention(); 

            });

            builder.Entity<HospitalDepartment>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "HospitalDepartments", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(dh => new { dh.HospitalId,  dh.DepartmentId });

                b.HasOne<Department>()
                    .WithMany(d => d.HospitalDepartments)
                    .HasForeignKey(dh => dh.DepartmentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<Hospital>()
                    .WithMany(h => h.HospitalDepartments)
                    .HasForeignKey(dh => dh.HospitalId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(dh => new { dh.HospitalId , dh.DepartmentId });

            });
        } 

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(HealthCareConsts.DbTablePrefix + "YourEntities", HealthCareConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}