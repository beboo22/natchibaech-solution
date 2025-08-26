using System.ComponentModel.DataAnnotations;

namespace Travelsite.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }

    public class CreateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
