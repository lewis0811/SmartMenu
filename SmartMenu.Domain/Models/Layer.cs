using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Layer : BaseModel
    {
        public int LayerId { get; set; }
        public int TemplateId { get; set; }

        [Required]
        public string LayerName { get; set; } = string.Empty;

        public LayerType LayerType { get; set; }

        //[ForeignKey(nameof(TemplateId))]
        //public Template? Template { get; set; }


        public LayerItem? LayerItem { get; set; }

        public ICollection<Box>? Boxes { get; set; }
    }
}