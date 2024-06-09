using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductGroupUpdateDTO
    {
        [Required]
        public string ProductGroupName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ProductGroupMaxCapacity { get; set; }
    }
}
