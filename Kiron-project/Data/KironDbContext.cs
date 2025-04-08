using Microsoft.EntityFrameworkCore;

public class KironDbContext : DbContext
{
    private readonly IAppSettingsService _appSettingsService;

    public KironDbContext(DbContextOptions<KironDbContext> options, IAppSettingsService appSettingsService)
        : base(options)
    {
        _appSettingsService = appSettingsService;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Navigation> Navigation { get; set; }
    public DbSet<BankHoliday> BankHoliday { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IAppSettings appSettings = _appSettingsService.GetAppSettings();
        optionsBuilder.UseSqlServer(appSettings.DatabaseConnection, options =>
        {
            options.MaxBatchSize(10);
        });

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<BankHoliday>(entity =>
        {
            entity.ToTable("BankHoliday");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Navigation>(entity =>
        {
            entity.ToTable("Navigation");
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade loops
        });
    }
}
