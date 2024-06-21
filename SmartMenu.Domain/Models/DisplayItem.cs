using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class DisplayItem : BaseModel
    {
        public int DisplayItemId { get; set; }
        public int DisplayId { get; set; }
        public int BoxId { get; set; }
        public int ProductGroupId { get; set; }

        [ForeignKey("DisplayId")]
        public Display? Display { get; set; }

        [ForeignKey("BoxId")]
        public Box? Box { get; set; }

        [ForeignKey("ProductGroupId")]
        public ProductGroup? ProductGroup { get; set; }
    }
}