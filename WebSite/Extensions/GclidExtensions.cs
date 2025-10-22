using Microsoft.EntityFrameworkCore;
using WebSite.Data.Models;

namespace WebSite.Extensions
{
    public static class GclidExtensions
    {
        public static async Task<GClid?> AddGclidIfPresentAsync(
            this DbContext db,
            string? gcLid,
            Data.Models.WebSite webSite,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(gcLid) || webSite is null)
                return null;

            var entry = new GClid
            {
                GoogleClickID = gcLid,
                WebSiteId = webSite.Id,
                MarketSubcatagory = webSite.MarketSubcatagory,
                CreateDate = DateTime.UtcNow
            };

            db.Set<GClid>().Add(entry);
            await db.SaveChangesAsync(ct);
            return entry;
        }
    }
}