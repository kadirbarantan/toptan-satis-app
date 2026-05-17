namespace ToptanSatis.API.DTOs
{
    public class ProductCreateDto
    {
        public string UrunAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
        public string? ResimUrl { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ProductUpdateDto
    {
        public string UrunAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
        public string? ResimUrl { get; set; }
        public int? CategoryId { get; set; }
        public bool AktifMi { get; set; }
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int Stok { get; set; }
        public string? ResimUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? KategoriAdi { get; set; }
        public bool AktifMi { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }
}