using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class Menu : BaseModel
    {
        public int MenuID { get; set; }
        public int BrandID { get; set; }

        [Required]
        public string MenuName { get; set; } = string.Empty;

        public string MenuDescription { get; set; } = string.Empty;

        //[ForeignKey(nameof(BrandID))]
        //public Brand? Brand { get; set; }

        public ICollection<ProductGroup>? ProductGroups { get; set; }

    }
}