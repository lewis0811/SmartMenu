using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Template : BaseModel
    {
        public int TemplateId { get; set; }
        public int BrandId { get; set; }

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string? TemplateDescription { get; set; }

        public float TemplateWidth { get; set; }
        public float TemplateHeight { get; set; }
        public TemplateType TemplateType { get; set; }
        public string? TemplateImgPath { get; set; }


        //[ForeignKey(nameof(BrandId))]
        //public Brand? Brand { get; set; }

        public ICollection<Layer>? Layers { get; set; }
    }
}