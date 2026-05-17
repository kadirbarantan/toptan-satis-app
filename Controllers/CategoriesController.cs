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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/categories
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    KategoriAdi = c.KategoriAdi,
                    Aciklama = c.Aciklama,
                    UrunSayisi = c.Products.Count(p => p.AktifMi)
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (await _context.Categories
                .AnyAsync(c => c.KategoriAdi == dto.KategoriAdi))
                return BadRequest("Bu kategori zaten mevcut.");

            var category = new Category
            {
                KategoriAdi = dto.KategoriAdi,
                Aciklama = dto.Aciklama
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kategori başarıyla eklendi.", id = category.Id });
        }

        // PUT api/categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CategoryCreateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Kategori bulunamadı.");

            category.KategoriAdi = dto.KategoriAdi;
            category.Aciklama = dto.Aciklama;

            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kategori başarıyla güncellendi." });
        }

        // DELETE api/categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound("Kategori bulunamadı.");

            if (category.Products.Any(p => p.AktifMi))
                return BadRequest("İçinde aktif ürün olan kategori silinemez.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Kategori başarıyla silindi." });
        }
    }
}