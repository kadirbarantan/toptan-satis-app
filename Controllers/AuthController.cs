using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToptanSatis.API.Data;
using ToptanSatis.API.DTOs;
using ToptanSatis.API.Models;
using ToptanSatis.API.Services;

namespace ToptanSatis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Email daha önce kayıtlı mı?
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Bu email zaten kayıtlı.");

            var user = new User
            {
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                Email = dto.Email,
                Sifre = BCrypt.Net.BCrypt.HashPassword(dto.Sifre),
                Telefon = dto.Telefon,
                Adres = dto.Adres,
                Sehir = dto.Sehir,
                Rol = dto.Rol
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.TokenOlustur(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Email = user.Email,
                Rol = user.Rol
            });
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Email veya şifre hatalı.");

            // Aktif mi?
            if (!user.AktifMi)
                return Unauthorized("Hesabınız aktif değil.");

            // Şifre doğru mu?
            if (!BCrypt.Net.BCrypt.Verify(dto.Sifre, user.Sifre))
                return Unauthorized("Email veya şifre hatalı.");

            var token = _jwtService.TokenOlustur(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Email = user.Email,
                Rol = user.Rol
            });
        }
    }
}