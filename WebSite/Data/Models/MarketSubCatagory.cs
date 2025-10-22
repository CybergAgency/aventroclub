namespace WebSite.Data.Models
{
    public class MarketSubCatagory : BaseEntity
    {
        public string Name { get; set; }
        public List<Brand> Brands { get; set; }
        public List<WebSite> WebSites { get; set; }
        public AppUser AppUser { get; set; }
        public Market Market { get; set; }
        public bool BlackSideStatus { get; set; } = false;
        public bool HighestBonus { get; set; } = true;
        public bool TopSlots { get; set; } = true;
        public bool TopPokers { get; set; }
        public bool Slider { get; set; }
    }
}