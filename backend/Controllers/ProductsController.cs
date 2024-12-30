using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Data;
using ProductManagementAPI.Models;
using System.Linq;

namespace ProductManagementAPI.Controllers
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

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                    .Where(p => !p.IsDeleted) // Filtra produtos não excluídos
                    .ToListAsync();

                if (products == null || !products.Any())
                {
                    return NotFound("Nenhum produto encontrado.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar produtos: {ex.Message}");
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se a categoria existe
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("A categoria especificada não existe.");
            }

            // Verifica se os fornecedores estão corretos
            foreach (var supplier in product.ProductSuppliers)
            {
                var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == supplier.SuppliersId);
                if (!supplierExists)
                {
                    return BadRequest($"O fornecedor com ID {supplier.SuppliersId} não existe.");
                }
            }

            try
            {
                // Adiciona o produto
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Adiciona as relações de ProductSupplier
                foreach (var supplier in product.ProductSuppliers)
                {
                    var productSupplier = new ProductSupplier
                    {
                        ProductsId = product.Id,
                        SuppliersId = supplier.SuppliersId
                    };
                    _context.ProductSuppliers.Add(productSupplier);
                }
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
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

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("ID do produto não corresponde.");
            }

            var existingProduct = await _context.Products
                .Include(p => p.ProductSuppliers)
                .ThenInclude(ps => ps.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
            {
                return NotFound("Produto não encontrado.");
            }

            // Verifica se a categoria existe
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("A categoria especificada não existe.");
            }

            // Verifica se os fornecedores estão corretos
            foreach (var supplier in product.ProductSuppliers)
            {
                var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == supplier.SuppliersId);
                if (!supplierExists)
                {
                    return BadRequest($"O fornecedor com ID {supplier.SuppliersId} não existe.");
                }
            }

            try
            {
                // Atualiza as propriedades do produto
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;

                // Atualiza a relação de fornecedores
                _context.ProductSuppliers.RemoveRange(existingProduct.ProductSuppliers);
                foreach (var supplier in product.ProductSuppliers)
                {
                    var productSupplier = new ProductSupplier
                    {
                        ProductsId = product.Id,
                        SuppliersId = supplier.SuppliersId
                    };
                    _context.ProductSuppliers.Add(productSupplier);
                }

                await _context.SaveChangesAsync();
                return Ok(existingProduct);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Erro ao atualizar no banco de dados: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Excluir logicamente
            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
