namespace ToptanSatis.API.DTOs
{
    public class RegisterDto
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string? Telefon { get; set; }
        public string? Adres { get; set; }
        public string? Sehir { get; set; }
        public string Rol { get; set; } = "Perakendeci";
    }
}