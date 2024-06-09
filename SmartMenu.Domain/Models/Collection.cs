using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class Collection : BaseModel
    {
        public int CollectionID { get; set; }
        public int BrandID { get; set; }

        [Required]
        public string CollectionName { get; set; } = string.Empty;

        public string CollectionDescription { get; set; } = string.Empty;

        //[ForeignKey(nameof(BrandID))]
        //public Brand? Brand { get; set; }

        public ICollection<ProductGroup>? ProductGroups { get; set; }

    }
}