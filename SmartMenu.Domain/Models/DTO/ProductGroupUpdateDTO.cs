using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductGroupUpdateDTO
    {
        [Required]
        public string ProductGroupName { get; set; } = string.Empty;

        public bool HaveNormalPrice { get; set; }

    }
}
