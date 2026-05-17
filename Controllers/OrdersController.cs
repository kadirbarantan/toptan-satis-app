using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToptanSatis.API.Data;
using ToptanSatis.API.DTOs;
using ToptanSatis.API.Models;

namespace ToptanSatis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            // Token'dan kullanıcı id'sini al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (dto.Urunler == null || !dto.Urunler.Any())
                return BadRequest("Sipariş en az 1 ürün içermelidir.");

            decimal toplamTutar = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Urunler)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.AktifMi);

                if (product == null)
                    return BadRequest($"Ürün bulunamadı: {item.ProductId}");

                if (product.Stok < item.Adet)
                    return BadRequest($"Yetersiz stok: {product.UrunAdi}");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Adet = item.Adet,
                    BirimFiyat = product.Fiyat
                };

                // Stok düş
                product.Stok -= item.Adet;
                toplamTutar += product.Fiyat * item.Adet;
                orderItems.Add(orderItem);
            }

            var order = new Order
            {
                UserId = userId,
                ToplamTutar = toplamTutar,
                SiparisNotu = dto.SiparisNotu,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sipariş başarıyla oluşturuldu.", id = order.Id });
        }

        // GET api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var rol = User.FindFirst(ClaimTypes.Role)!.Value;

            // Admin tüm siparişleri görür, Perakendeci sadece kendinkini
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (rol != "Admin")
                query = query.Where(o => o.UserId == userId);

            var orders = await query
                .OrderByDescending(o => o.Tarih)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    MusteriAd = o.User!.Ad,
                    MusteriSoyad = o.User.Soyad,
                    Tarih = o.Tarih,
                    ToplamTutar = o.ToplamTutar,
                    Durum = o.Durum,
                    SiparisNotu = o.SiparisNotu,
                    Urunler = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        ProductId = oi.ProductId,
                        UrunAdi = oi.Product!.UrunAdi,
                        Adet = oi.Adet,
                        BirimFiyat = oi.BirimFiyat,
                        Toplam = oi.Adet * oi.BirimFiyat
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET api/orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var rol = User.FindFirst(ClaimTypes.Role)!.Value;

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Sipariş bulunamadı.");

            // Perakendeci sadece kendi siparişini görebilir
            if (rol != "Admin" && order.UserId != userId)
                return Forbid();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                MusteriAd = order.User!.Ad,
                MusteriSoyad = order.User.Soyad,
                Tarih = order.Tarih,
                ToplamTutar = order.ToplamTutar,
                Durum = order.Durum,
                SiparisNotu = order.SiparisNotu,
                Urunler = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    UrunAdi = oi.Product!.UrunAdi,
                    Adet = oi.Adet,
                    BirimFiyat = oi.BirimFiyat,
                    Toplam = oi.Adet * oi.BirimFiyat
                }).ToList()
            };

            return Ok(response);
        }

        // PUT api/orders/5/durum
        [HttpPut("{id}/durum")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDurum(int id, OrderDurumUpdateDto dto)
        {
            var gecerliDurumlar = new[] { "Bekliyor", "Onaylandi", "Hazirlaniyor", "Yolda", "TeslimEdildi", "Iptal" };

            if (!gecerliDurumlar.Contains(dto.Durum))
                return BadRequest("Geçersiz sipariş durumu.");

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound("Sipariş bulunamadı.");

            order.Durum = dto.Durum;
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Sipariş durumu güncellendi.", durum = order.Durum });
        }
    }
}