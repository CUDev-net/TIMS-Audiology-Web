using Microsoft.EntityFrameworkCore;
using TIMS_X.Server.Models;

namespace TIMS_X.Server.Data;

/// <summary>
///     DbContext for the TIMSInternal database
/// </summary>
public class TimsInternalDbContext : DbContext
{
    public TimsInternalDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<ApiUrl> ApiUrls { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<SupportUser> SupportUsers { get; set; }
    public virtual DbSet<TimsServer> TimsServers { get; set; }
    public virtual DbSet<VendorPermission> VendorPermissions { get; set; }
    public virtual DbSet<Vendor> Vendors { get; set; }
    public virtual DbSet<FormLink> FormLinks { get; set; }
    public virtual DbSet<TimsLog> TimsLogs { get; set; }


	/// <summary>
	///     Here we map all the tables out. In other db contexts, the mapping is done in ModelConfigurator.cs and shared among
	///     data services.
	///     Since we only ever need one db context to connect to TIMSInternal, the mapping is done here. The mapping was
	///     originally generated with
	///     the command "dotnet ef dbcontext scaffold" then copied over
	/// </summary>
	/// <param name="modelBuilder"></param>
	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.OfficeCode)
                .HasDatabaseName("UQ_Customer_OfficeCode")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Database)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.Property(e => e.SqlUser)
                .HasMaxLength(100)
                .IsUnicode();

            entity.Property(e => e.SqlPassword)
                .HasMaxLength(100)
                .IsUnicode();

            entity.Property(e => e.TimeZoneId)
                .HasConversion<int>();

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.DateUpdated).HasColumnType("datetime");

            entity.Property(e => e.Name).HasMaxLength(128);

            entity.Property(e => e.Notes).HasMaxLength(256);

            entity.Property(e => e.OfficeCode)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.HasOne(d => d.Server)
                .WithMany(p => p.Customers)
                .HasForeignKey(d => d.ServerId)
                .HasConstraintName("FK_Customer_TimsServer");

            entity.HasOne(d => d.UpdatedByUser)
                .WithMany(e => e.CustomersUpdated)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Customer_User");
            entity.Ignore(e => e.PermissionList);

            entity.ToTable(nameof(Customer));
        });

        modelBuilder.Entity<SupportUser>(entity =>
        {
            entity.HasIndex(e => e.Email)
                .HasDatabaseName("UQ_SupportUser_Email")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");


            entity.ToTable(nameof(SupportUser));
        });

        modelBuilder.Entity<TimsServer>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("UQ_TimsServer_Name")
                .IsUnique();

            entity.HasIndex(e => e.Address)
                .HasDatabaseName("UQ_TimsServer_Address")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.ToTable(nameof(TimsServer));
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasIndex(e => e.ApiKey)
                .HasDatabaseName("UQ_Vendor_ApiKey")
                .IsUnique();

            entity.HasIndex(e => e.Name)
                .HasDatabaseName("UQ_Vendor_Name")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ApiKey)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.Ignore(e => e.DefaultPermissionsJson);

            entity.ToTable(nameof(Vendor));
        });

        modelBuilder.Entity<ApiUrl>(entity =>
        {
            entity.HasIndex(e => e.Url)
                .HasDatabaseName("UQ_ApiUrl_Url")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.ToTable(nameof(ApiUrl));
        });

        modelBuilder.Entity<CustomerVendorPermission>(entity =>
        {
            entity.HasKey(e => new { e.CustomerId, e.VendorId, e.PermissionId });

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.VendorPermissions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CVP_Customer");

            entity.HasOne(d => d.Permission)
                .WithMany(d => d.AssociatedCustomers)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CVP_Permission");

            entity.HasOne(d => d.Vendor)
                .WithMany(p => p.CustomerPermissions)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CVP_Vendor");

            entity.ToTable(nameof(CustomerVendorPermission));
        });

        modelBuilder.Entity<VendorPermission>(entity =>
        {
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("UQ_VendorPermission_Name")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.Ignore(e => e.UrlsJson);

            entity.ToTable(nameof(VendorPermission));
        });

        modelBuilder.Entity<VendorPermissionApiUrl>(entity =>
        {
            entity.HasKey(e => new { e.PermissionId, e.ApiUrlId });

            entity.HasOne(d => d.ApiUrl)
                .WithMany()
                .HasForeignKey(d => d.ApiUrlId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_VPAU_ApiUrl");

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.ApiUrls)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_VPAU_Permission");

            entity.ToTable(nameof(VendorPermissionApiUrl));
        });

        modelBuilder.Entity<DefaultVendorPermission>(entity =>
        {
            entity.HasKey(e => new { e.VendorId, e.PermissionId });

            entity.HasOne(d => d.Permission)
                .WithMany(d => d.AssociatedDefaultPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DVP_Permission");

            entity.HasOne(d => d.Vendor)
                .WithMany(p => p.DefaultPermissions)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DVP_Vendor");
            entity.ToTable(nameof(DefaultVendorPermission));
        });

        modelBuilder.Entity<FormLink>(entity =>
        {
            entity.HasIndex(e => e.Url)
                .HasDatabaseName("UQ_FormLink_Url")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Url)
                .HasMaxLength(6)
                .IsUnicode(false);

            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.FormLinks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormLink_Customer");

            entity.ToTable(nameof(FormLink));
        });

        modelBuilder.Entity<TimsLog>(entity =>
        {
	        entity.Property(e => e.Id).HasColumnName("ID");

	        entity.Property(e => e.OfficeCode)
		        .HasMaxLength(128);

			entity.Property(e => e.Message)
		        .HasMaxLength(256);
	        entity.Property(e => e.Error)
		        .HasMaxLength(4000);

	        entity.Property(e => e.DateCreated)
		        .HasColumnType("datetime")
		        .HasDefaultValueSql("(getdate())");

	        entity.ToTable(nameof(TimsLog));
        });
	}
}