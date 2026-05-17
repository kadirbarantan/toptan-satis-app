namespace ToptanSatis.API.DTOs
{
    public class CategoryCreateDto
    {
        public string KategoriAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public int UrunSayisi { get; set; }
    }
}