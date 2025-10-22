namespace WebSite.Data.Models
{
    public class Log : BaseEntity
    {
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public WebSite WebSite { get; set; }
        public int WebSiteId { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public GClid? GClid { get; set; }
        public int? GClidId { get; set; }
        public bool IsBlack { get; set; }
        public MarketSubCatagory MarketSubcatagory { get; set; }
        public int MarketSubcatagoryId { get; set; }
    }
}