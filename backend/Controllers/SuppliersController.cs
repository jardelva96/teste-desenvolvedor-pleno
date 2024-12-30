using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Data;
using ProductManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;


namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        [AllowAnonymous] // Permite acesso público
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var suppliers = await _context.Suppliers
                    .Include(s => s.ProductSuppliers)
                    .ThenInclude(ps => ps.Product)
                    .ToListAsync();

                if (suppliers == null || !suppliers.Any())
                {
                    return NotFound("Nenhum fornecedor encontrado.");
                }

                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar fornecedores: {ex.Message}");
            }
        }

        // POST: api/Suppliers
        [HttpPost]
        public async Task<IActionResult> PostSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSuppliers), new { id = supplier.Id }, supplier);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Erro ao salvar no banco de dados: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
