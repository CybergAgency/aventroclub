using WebSite.Data.Models;

namespace WebSite.Dtos
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Url { get; set; }
        public int Place { get; set; }
        public PostBackType PostBackType { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Description { get; set; } = null;
        public List<string> PaymentOptions { get; set; }
        public int WebSiteId { get; set; }

        public int MarketSubcatagoryId { get; set; }
    }
}