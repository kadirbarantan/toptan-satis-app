using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToptanSatis.API.Data;
using ToptanSatis.API.DTOs;
using ToptanSatis.API.Models;

namespace ToptanSatis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/products
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.AktifMi)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    UrunAdi = p.UrunAdi,
                    Aciklama = p.Aciklama,
                    Fiyat = p.Fiyat,
                    Stok = p.Stok,
                    ResimUrl = p.ResimUrl,
                    CategoryId = p.CategoryId,
                    KategoriAdi = p.Category != null ? p.Category.KategoriAdi : null,
                    AktifMi = p.AktifMi,
                    OlusturulmaTarihi = p.OlusturulmaTarihi
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET api/products/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id && p.AktifMi)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    UrunAdi = p.UrunAdi,
                    Aciklama = p.Aciklama,
                    Fiyat = p.Fiyat,
                    Stok = p.Stok,
                    ResimUrl = p.ResimUrl,
                    CategoryId = p.CategoryId,
                    KategoriAdi = p.Category != null ? p.Category.KategoriAdi : null,
                    AktifMi = p.AktifMi,
                    OlusturulmaTarihi = p.OlusturulmaTarihi
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound("Ürün bulunamadı.");

            return Ok(product);
        }

        // POST api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var product = new Product
            {
                UrunAdi = dto.UrunAdi,
                Aciklama = dto.Aciklama,
                Fiyat = dto.Fiyat,
                Stok = dto.Stok,
                ResimUrl = dto.ResimUrl,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Ürün başarıyla eklendi.", id = product.Id });
        }

        // PUT api/products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Ürün bulunamadı.");

            product.UrunAdi = dto.UrunAdi;
            product.Aciklama = dto.Aciklama;
            product.Fiyat = dto.Fiyat;
            product.Stok = dto.Stok;
            product.ResimUrl = dto.ResimUrl;
            product.CategoryId = dto.CategoryId;
            product.AktifMi = dto.AktifMi;

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Ürün başarıyla güncellendi." });
        }

        // DELETE api/products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Ürün bulunamadı.");

            // Fiziksel silme yerine pasife alma
            product.AktifMi = false;
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Ürün başarıyla silindi." });
        }
    }
}