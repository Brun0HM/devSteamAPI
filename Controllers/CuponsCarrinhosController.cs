using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using devSteamAPI.Data;
using devSteamAPI.Models;

namespace devSteamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponsCarrinhosController : ControllerBase
    {
        private readonly APIContext _context;

        public CuponsCarrinhosController(APIContext context)
        {
            _context = context;
        }

        // GET: api/CuponsCarrinhos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CupomCarrinho>>> GetCuponsCarrinhos()
        {
            return await _context.CuponsCarrinhos.ToListAsync();
        }

        // GET: api/CuponsCarrinhos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CupomCarrinho>> GetCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CuponsCarrinhos.FindAsync(id);

            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            return cupomCarrinho;
        }

        // PUT: api/CuponsCarrinhos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCupomCarrinho(Guid id, CupomCarrinho cupomCarrinho)
        {
            if (id != cupomCarrinho.CupomCarrinhoId)
            {
                return BadRequest();
            }

            _context.Entry(cupomCarrinho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CupomCarrinhoExists(id))
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

        // POST: api/CuponsCarrinhos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CupomCarrinho>> PostCupomCarrinho(CupomCarrinho cupomCarrinho)
        {
            _context.CuponsCarrinhos.Add(cupomCarrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCupomCarrinho", new { id = cupomCarrinho.CupomCarrinhoId }, cupomCarrinho);
        }

        // DELETE: api/CuponsCarrinhos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCupomCarrinho(Guid id)
        {
            var cupomCarrinho = await _context.CuponsCarrinhos.FindAsync(id);
            if (cupomCarrinho == null)
            {
                return NotFound();
            }

            _context.CuponsCarrinhos.Remove(cupomCarrinho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CupomCarrinhoExists(Guid id)
        {
            return _context.CuponsCarrinhos.Any(e => e.CupomCarrinhoId == id);
        }
        [HttpPut("AplicarCupom")]
        public async Task<IActionResult> AplicarCupom(Guid carrinhoId, string codigoCupom)
        {
            // Validar se o carrinho existe
            var carrinho = await _context.Carrinhos
                .Include(c => c.ItensCarrinhos)
                .FirstOrDefaultAsync(c => c.CarrinhoId == carrinhoId);

            if (carrinho == null)
                return NotFound("Carrinho não encontrado.");

            if (carrinho.Finalizado == true)
                return BadRequest("O carrinho já foi finalizado.");

            // Validar se o cupom existe
            var cupom = await _context.cupons.FirstOrDefaultAsync(c => c.Nome == codigoCupom);

            if (cupom == null)
                return NotFound("Cupom não encontrado.");

            // Validar se o cupom está ativo
            if (cupom.Ativo != true)
                return BadRequest("O cupom não está ativo.");

            // Validar se o cupom não expirou
            if (cupom.DataValidade.HasValue && cupom.DataValidade.Value < DateTime.UtcNow)
                return BadRequest("O cupom expirou.");

            // Validar se o cupom não excedeu o limite de uso
            var usosDoCupom = await _context.CuponsCarrinhos.CountAsync(cc => cc.CupomId == cupom.CupomId);
            if (cupom.LimiteUso.HasValue && usosDoCupom >= cupom.LimiteUso.Value)
                return BadRequest("O cupom atingiu o limite máximo de utilizações.");

            // Aplicar o desconto ao valor total do carrinho
            var desconto = cupom.Desconto / 100.0m;
            carrinho.ValorTotal -= carrinho.ValorTotal * desconto;

            // Atualizar o carrinho no banco de dados
            try
            {
                _context.Entry(carrinho).State = EntityState.Modified;

                // Registrar o uso do cupom
                var cupomCarrinho = new CupomCarrinho
                {
                    CupomCarrinhoId = Guid.NewGuid(),
                    CarrinhoId = carrinho.CarrinhoId,
                    CupomId = cupom.CupomId,
                    Expirado = false
                };
                _context.CuponsCarrinhos.Add(cupomCarrinho);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao aplicar o cupom: {ex.Message}");
            }

            // Retornar o carrinho atualizado
            return Ok(carrinho);
        }
    }
}
