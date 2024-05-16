using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class Template : BaseModel
    {
        public int TemplateID { get; set; }
        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string TemplateDescription { get; set; } = string.Empty;
        public ICollection<Layer>? Layers { get; set; }
    }
}