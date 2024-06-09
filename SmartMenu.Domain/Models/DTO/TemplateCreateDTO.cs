using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class TemplateCreateDTO
    {
        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string TemplateDescription { get; set; } = string.Empty;
    }
}
