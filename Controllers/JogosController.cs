using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using devSteamAPI.Data;
using devSteamAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace devSteamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JogosController(APIContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jogo>>> GetJogos()
        {
            return await _context.Jogos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Jogo>> GetJogo(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);

            if (jogo == null)
            {
                return NotFound();
            }

            return jogo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutJogo(Guid id, Jogo jogo)
        {
            if (id != jogo.JogoId)
            {
                return BadRequest();
            }

            _context.Entry(jogo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JogoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("UploadGameBanner")]
        public async Task<IActionResult> UploadGameBanner(IFormFile file, Guid jogoId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não pode ser nulo ou vazio.");

            var jogo = await _context.Jogos.FindAsync(jogoId);
            if (jogo == null)
                return NotFound("Jogo não encontrado.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("O arquivo deve ser uma imagem.");

            var gamesFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Games");
            if (!Directory.Exists(gamesFolder))
                Directory.CreateDirectory(gamesFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) < 0)
                return BadRequest("Formato de arquivo não suportado. Use .jpg, .jpeg, .png ou .gif.");

            var fileName = $"{jogo.JogoId}{fileExtension}";
            var filePath = Path.Combine(gamesFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("Resources", "Games", fileName).Replace("\\", "/");
            return Ok(new { FilePath = relativePath });
        }

        [HttpPost]
        public async Task<ActionResult<Jogo>> PostJogo(Jogo jogo)
        {
            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJogo", new { id = jogo.JogoId }, jogo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJogo(Guid id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
            {
                return NotFound();
            }

            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JogoExists(Guid id)
        {
            return _context.Jogos.Any(e => e.JogoId == id);
        }
    }
}