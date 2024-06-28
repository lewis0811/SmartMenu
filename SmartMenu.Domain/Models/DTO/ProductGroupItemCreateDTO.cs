using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductGroupItemCreateDTO
    {
        [Required]
        public int ProductGroupId { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
