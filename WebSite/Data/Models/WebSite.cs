namespace WebSite.Data.Models
{
    public class WebSite : BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public MarketSubCatagory? MarketSubcatagory { get; set; }
        public List<Log> Logs { get; set; }
    }
}