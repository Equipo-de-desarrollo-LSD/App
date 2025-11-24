using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using WayFinder.Calificaciones;

namespace WayFinder.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class WayFinderDbContext :
    AbpDbContext<WayFinderDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Calificacion> Calificaciones { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */
    public DbSet<DestinoTuristico> DestinosTuristicos { get; set; } //agregue por chat

    // Identity
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

    public WayFinderDbContext(DbContextOptions<WayFinderDbContext> options)
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
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        
        /* Configure your own tables/entities inside here */
        builder.Entity<DestinoTuristico>(b =>
        {
            b.ToTable(WayFinderConsts.DbTablePrefix + "DestinosTuristicos", WayFinderConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.nombre).IsRequired().HasMaxLength(128);
            b.Property(x => x.foto).IsRequired().HasMaxLength(512);
            b.Property(x => x.UltimaActualizacion).IsRequired();
            b.OwnsOne(x => x.Pais, pais =>
            {
                pais.Property(p => p.nombre).IsRequired().HasMaxLength(64).HasColumnName("pais_nombre");
                pais.Property(p => p.poblacion).IsRequired().HasColumnName("pais_poblacion");
            });
            b.OwnsOne(x => x.Coordenadas, c =>
            {
                c.Property(cp => cp.latitud).IsRequired().HasColumnName("coordenadas_latitud");
                c.Property(cp => cp.longitud).IsRequired().HasColumnName("coordenadas_longitud");
            });
            //b.HasMany(x => x.Reviews).WithOne().HasForeignKey(x => x.DestinoTuristicoId);
            //...
        });

        builder.Entity<Calificacion>(b =>
        {
            // 1. Configura el nombre de la tabla (igual que DestinoTuristico)
            b.ToTable(WayFinderConsts.DbTablePrefix + "Calificaciones", WayFinderConsts.DbSchema);

            // 2. Configura las propiedades base (Id, CreationTime, etc.)
            b.ConfigureByConvention();

            // 3. Configura tus propiedades
            b.Property(x => x.Puntaje).IsRequired();
            b.Property(x => x.Comentario).HasMaxLength(512); // Siempre es bueno poner un límite

            // 4. Configura la relación con el Destino
            b.HasOne<DestinoTuristico>().WithMany()
                .HasForeignKey(x => x.DestinoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Opcional: si borras un destino, se borran sus calificaciones

            // 5. Configura la relación con el Usuario (de IUserOwned)
            b.HasOne<IdentityUser>().WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction); // No borrar usuarios si se borra una calificación
        });

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(WayFinderConsts.DbTablePrefix + "YourEntities", WayFinderConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}
