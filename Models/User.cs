namespace ToptanSatis.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string? Telefon { get; set; }
        public string? Adres { get; set; }
        public string? Sehir { get; set; }
        public string Rol { get; set; } = "Perakendeci";
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
        public bool AktifMi { get; set; } = true;

        // Navigation property
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}