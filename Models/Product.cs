namespace ToptanSatis.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; } = 0;
        public string? ResimUrl { get; set; }
        public int? CategoryId { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        public bool AktifMi { get; set; } = true;

        // Navigation properties
        public Category? Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}