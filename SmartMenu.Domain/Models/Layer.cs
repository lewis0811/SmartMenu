using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Layer : BaseModel
    {
        public int LayerID { get; set; }
        public int TemplateID { get; set; }

        [Required]
        public string LayerName { get; set; } = string.Empty;

        //[ForeignKey("TemplateID")]
        //public Template? Template { get; set; }

        public ICollection<LayerItem>? LayerItems { get; set; }
        public ICollection<Box>? Boxes { get; set; }
    }
}