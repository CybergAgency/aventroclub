namespace WebSite.Data.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow.AddHours(4);
        public bool IsActive { get; set; } = true;
    }
}