namespace WebSite.Dtos
{
    public class IndexDto
    {
        public bool Status { get; set; } = false;
        public List<BrandDto>? Brands { get; set; }
    }
}