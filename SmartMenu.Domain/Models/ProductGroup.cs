using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductGroup : BaseModel
    {
        public int ProductGroupID { get; set; }
        public int MenuID { get; set; }
        public int CollectionID { get; set; }

        [Required]
        public string ProductGroupName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ProductGroupMaxCapacity { get; set; }

        //[ForeignKey(nameof(MenuID))]
        //public Menu? Menu { get; set; }

        //[ForeignKey(nameof(CollectionID))]
        //public Collection? Collection { get; set; }

        public ICollection<ProductGroupItem>? ProductGroupItems { get; set; }
    }
}