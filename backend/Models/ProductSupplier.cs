using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementAPI.Models
{
    public class ProductSupplier
    {
        [Key]
        [Column(Order = 1)]  // Ordem da chave composta
        public int ProductsId { get; set; }

        public Product? Product { get; set; }  // Tornando o Product anulável

        [Key]
        [Column(Order = 2)]  // Ordem da chave composta
        public int SuppliersId { get; set; }

        public Supplier? Supplier { get; set; }  // Tornando o Supplier anulável
    }
}
