using GeraFin.Models.Logging;
using GeraFin.Models.Importer;
using GeraFin.Models.WebClient;
using Microsoft.EntityFrameworkCore;
using GeraFin.Models.ViewModels.Dashboard;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GeraFin.Models.SessionState;

namespace GeraFin.DAL.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("GeraFin");
            base.OnModelCreating(builder);
        }

        public DbSet<ApplicationDetails> ApplicationDetails { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<BranchGP> BranchGP { get; set; }
        public DbSet<NumberSequence> NumberSequence { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }
        public DbSet<PurchaseSummary> PurchaseSummary { get; set; }
        public DbSet<PurchaseSummaryLine> PurchaseSummaryLine { get; set; }
        public DbSet<StockTake> StockTake { get; set; }
        public DbSet<StockTakeLine> StockTakeLine { get; set; }
        public DbSet<WeeklySignOff> WeeklySignOff { get; set; }
        public DbSet<WeeklySignOffLine> WeeklySignOffLine { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLine { get; set; }
        public DbSet<PurchaseType> PurchaseType { get; set; }
        public DbSet<Specials> Specials { get; set; }
        public DbSet<DailyIncome> DailyIncome { get; set; }
        public DbSet<DailyIncomeLine> DailyIncomeLine { get; set; }
        public DbSet<DailyIssues> DailyIssues { get; set; }
        public DbSet<DailyIssuesLine> DailyIssuesLine { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasure { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }
        public DbSet<UserTasks> UserTasks { get; set; }
        public DbSet<EventsCalendar> EventsCalendar { get; set; }
        public DbSet<QuickEmail> QuickEmail { get; set; }
        public DbSet<InternalLog> InternalLog { get; set; }
        public DbSet<RequestLog> RequestLog { get; set; }
        public DbSet<ResponseLog> ResponseLog { get; set; }
        public DbSet<UserConfig> UserConfig { get; set; }
        public DbSet<UpdateFilesUploaded> UpdateFilesUploaded { get; set; }
        public DbSet<UpdateProductList> UpdateProductList { get; set; }
        public DbSet<CreditedLines> CreditedLines { get; set; }
        public DbSet<iUserSession> iUserSession { get; set; }
    }
}