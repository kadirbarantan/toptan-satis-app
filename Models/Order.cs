namespace ToptanSatis.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = "Bekliyor";
        public string? SiparisNotu { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}