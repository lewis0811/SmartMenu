using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductGroupItem : BaseModel
    {
        public int ProductGroupItemID { get; set; }
        public int ProductGroupID { get; set; }
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product? Product { get; set; }

        //[ForeignKey("ProductGroupID")]
        //public ProductGroup? ProductGroup { get; set; }
    }
}