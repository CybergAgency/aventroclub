namespace WebSite.Data.Models
{
    public class Brand : BaseEntity
    {
        public string? Logo { get; set; }
        public string? Link { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Place { get; set; }
        public int SlotPlace { get; set; }
        public double Bonus { get; set; }
        public string? BonusDesciription { get; set; }
        public bool TopSLot { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public MarketSubCatagory? MarketSubcatagory { get; set; }
        public int? MarketSubcatagoryId { get; set; }
        public List<PaymentType>? PaymentTypes { get; set; }
        public PostBackType? PostBackType { get; set; }
        public byte[]? LogoBase64 { get; set; }
    }

    public enum PostBackType
    {
        Raketech,
        Affilika
    }
}