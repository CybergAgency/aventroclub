namespace WebSite.Data.Models
{
    public class BrandClick : BaseEntity
    {
        public WebSite WebSite { get; set; }
        public int WebSiteId { get; set; }
        public int GClidId { get; set; }
        public GClid GClid { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int Place { get; set; }
        public MarketSubCatagory MarketSubcatagory { get; set; }
        public int MarketSubcatagoryId { get; set; }
    }
}