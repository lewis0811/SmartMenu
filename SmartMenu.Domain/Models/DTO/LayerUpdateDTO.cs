using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerUpdateDTO
    {
        [Required]
        public string LayerName { get; set; } = string.Empty;
    }
}       
