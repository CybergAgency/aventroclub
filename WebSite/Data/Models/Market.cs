namespace WebSite.Data.Models
{
    public class Market : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Flag { get; set; }
        public string Code { get; set; }
        public List<MarketSubCatagory> MarketSubcatagories { get; set; }
        public string PlayNow { get; set; }
        public string License { get; set; }
        public string PayOut { get; set; }
        public string Country { get; set; }
    }
}