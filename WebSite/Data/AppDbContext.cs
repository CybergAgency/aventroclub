using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebSite.Data.Models;

namespace WebSite.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Models.WebSite> WebSites { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<MarketSubCatagory> MarketSubcatagories { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<GClid> GClids { get; set; }
        public DbSet<PostBack> PostBacks { get; set; }
        public DbSet<BrandClick> BrandClicks { get; set; }
        public DbSet<StaticData> StaticDatas { get; set; }
    }
}