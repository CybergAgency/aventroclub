namespace WebSite.Data.Models
{
    public class PaymentType : BaseEntity
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public byte[] LogoBase64 { get; set; }
        public List<Brand> Brands { get; set; }
    }
}