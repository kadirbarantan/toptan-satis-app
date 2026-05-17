namespace ToptanSatis.API.DTOs
{
    public class OrderItemCreateDto
    {
        public int ProductId { get; set; }
        public int Adet { get; set; }
    }

    public class OrderCreateDto
    {
        public string? SiparisNotu { get; set; }
        public List<OrderItemCreateDto> Urunler { get; set; } = new List<OrderItemCreateDto>();
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Toplam { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string MusteriAd { get; set; } = string.Empty;
        public string MusteriSoyad { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public decimal ToplamTutar { get; set; }
        public string Durum { get; set; } = string.Empty;
        public string? SiparisNotu { get; set; }
        public List<OrderItemResponseDto> Urunler { get; set; } = new List<OrderItemResponseDto>();
    }

    public class OrderDurumUpdateDto
    {
        public string Durum { get; set; } = string.Empty;
    }
}