namespace WebSite.Data.Models
{
    public class PostBack : BaseEntity
    {
        public GClid GClid { get; set; }
        public string CampaignID { get; set; }
        public string PB_Type { get; set; }
        public string Brand { get; set; }
        public string Cuurency { get; set; }
        public string Amount { get; set; }
    }
}