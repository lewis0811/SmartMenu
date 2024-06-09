using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class DisplayItem : BaseModel
    {
        public int DisplayItemID { get; set; }
        public int DisplayID { get; set; }
        public int BoxID { get; set; }
        public int ProductGroupID { get; set; }

        [ForeignKey("DisplayID")]
        public Display? Display { get; set; }

        [ForeignKey("BoxID")]
        public Box? Box { get; set; }

        [ForeignKey("ProductGroupID")]
        public ProductGroup? ProductGroup { get; set; }
    }
}