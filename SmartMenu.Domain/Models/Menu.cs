using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Menu : BaseModel
    {
        public int MenuId { get; set; }
        public int BrandId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Menu name must be between 5 and 100 characters")]
        public string MenuName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Menu description cannot exceed 500 characters")]
        public string? MenuDescription { get; set; }
        //public string? MenuBackgroundImgPath { get; set; }

        //[ForeignKey(nameof(BrandId))]
        //public Brand? Brand { get; set; } //

        public ICollection<ProductGroup>? ProductGroups { get; set; }

    }
}