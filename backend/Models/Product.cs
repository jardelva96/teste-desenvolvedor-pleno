using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // Relacionamento com ProductSuppliers
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();

        // Adicionando campo IsDeleted para exclusão lógica
        public bool IsDeleted { get; set; } = false;
    }
}
