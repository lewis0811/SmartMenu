using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Menu : BaseModel
    {
        public int MenuId { get; set; }
        public int BrandId { get; set; }

        [Required]
        public string MenuName { get; set; } = string.Empty;

        public string? MenuDescription { get; set; }
        //public string? MenuBackgroundImgPath { get; set; }

        //[ForeignKey(nameof(BrandId))]
        //public Brand? Brand { get; set; } //

        public ICollection<ProductGroup>? ProductGroups { get; set; }

    }
}