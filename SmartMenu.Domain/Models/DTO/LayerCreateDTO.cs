using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerCreateDTO
    {
        public int TemplateID { get; set; }

        [Required]
        public string LayerName { get; set; } = string.Empty;
    }
}
