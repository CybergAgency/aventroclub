namespace WebSite.Data.Models
{
    public class GClid : BaseEntity
    {
        public string? GoogleClickID { get; set; }
        public int WebSiteId { get; set; }
        public WebSite? WebSite { get; set; }
        public int MarketSubcatagoryId { get; set; }
        public MarketSubCatagory? MarketSubcatagory { get; set; }
        public List<PostBack>? PostBacks { get; set; }
    }
}