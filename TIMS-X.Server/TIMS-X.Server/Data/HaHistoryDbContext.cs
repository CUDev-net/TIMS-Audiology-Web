using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.Server.Config;

namespace TIMS_X.Server.Data;

public class HaHistoryDbContext : DbContext
{
	public HaHistoryDbContext(DbContextOptions<HaHistoryDbContext> options) : base(options)
	{
	}

	public DbSet<BatterySize> BatterySizes { get; set; }
	public DbSet<HaAttachment> HaAttachments { get; set; }
	public DbSet<HaComponent> HaAttachmentTypes { get; set; }
	public DbSet<HaHistory> HaHistories { get; set; }
	public DbSet<HaHistoryOption> HaHistoryOptions { get; set; }
	public DbSet<HaLoaner> HaLoaners { get; set; }
	public DbSet<HaModelOption> HaModelOptions { get; set; }
	public DbSet<HaModel> HaModels { get; set; }
	public DbSet<HaOption> HaOptions { get; set; }
	public DbSet<HaOrderImage> HaOrderImages { get; set; }
	public DbSet<HaOrder> HaOrders { get; set; }
	public DbSet<HaReturnReason> HaReturnReasons { get; set; }
	public DbSet<HaReturn> HaReturns { get; set; }
	public DbSet<HaStatus> HaStatuses { get; set; }
	public DbSet<HaStockItemImage> HaStockItemImages { get; set; }
	public DbSet<HaStockItem> HaStockItems { get; set; }
	public DbSet<HaStockOption> HaStockOptions { get; set; }
	public DbSet<HaStyle> HaStyles { get; set; }
	public DbSet<HaType> HaTypes { get; set; }
	public DbSet<Manufacturer> Manufacturers { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ModelConfigurator.Configure(modelBuilder, typeof(BatterySize));
		ModelConfigurator.Configure(modelBuilder, typeof(Manufacturer));
		ModelConfigurator.Configure(modelBuilder, typeof(HaAttachment));
		ModelConfigurator.Configure(modelBuilder, typeof(HaComponent));
		ModelConfigurator.Configure(modelBuilder, typeof(HaHistory));
		ModelConfigurator.Configure(modelBuilder, typeof(HaHistoryOption));
		ModelConfigurator.Configure(modelBuilder, typeof(HaLoaner));
		ModelConfigurator.Configure(modelBuilder, typeof(HaModel));
		ModelConfigurator.Configure(modelBuilder, typeof(HaModelOption));
		ModelConfigurator.Configure(modelBuilder, typeof(HaOption));
		ModelConfigurator.Configure(modelBuilder, typeof(HaOrder));
		ModelConfigurator.Configure(modelBuilder, typeof(HaOrderImage));
		ModelConfigurator.Configure(modelBuilder, typeof(HaReturn));
		ModelConfigurator.Configure(modelBuilder, typeof(HaReturnReason));
		ModelConfigurator.Configure(modelBuilder, typeof(HaStatus));
		ModelConfigurator.Configure(modelBuilder, typeof(HaStockItem));
		ModelConfigurator.Configure(modelBuilder, typeof(HaStockItemImage));
		ModelConfigurator.Configure(modelBuilder, typeof(HaStockOption));
		ModelConfigurator.Configure(modelBuilder, typeof(HaStyle));
		ModelConfigurator.Configure(modelBuilder, typeof(HaType));
	}
}