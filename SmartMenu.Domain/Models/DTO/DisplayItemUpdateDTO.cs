using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class DisplayItemUpdateDTO
    {
        [Required(ErrorMessage = "Box ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Box ID must be a positive integer.")]
        public int BoxID { get; set; }

        [Required(ErrorMessage = "Product Group ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product Group ID must be a positive integer.")]
        public int ProductGroupID { get; set; }
    }
}