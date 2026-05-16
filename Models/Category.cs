namespace ToptanSatis.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}