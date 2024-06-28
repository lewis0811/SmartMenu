using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductGroup : BaseModel
    {
        public int ProductGroupId { get; set; }

        public int? MenuId { get; set; }

        public int? CollectionId { get; set; }

        [Required]
        public string ProductGroupName { get; set; } = string.Empty;

        //[ForeignKey(nameof(MenuId))]
        //public Menu? Menu { get; set; }

        //[ForeignKey(nameof(CollectionId))]
        //public Collection? Collection { get; set; }

        public ICollection<ProductGroupItem>? ProductGroupItems { get; set; }
    }

}