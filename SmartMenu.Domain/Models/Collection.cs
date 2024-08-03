using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Collection : BaseModel
    {
        public int CollectionId { get; set; }
        public int BrandId { get; set; }

        [Required]
        public string CollectionName { get; set; } = string.Empty;

        public string? CollectionDescription { get; set; }
        public string? CollectionBackgroundImgPath { get; set; }

        [ForeignKey(nameof(BrandId))]
        public Brand? Brand { get; set; } //

        public ICollection<ProductGroup>? ProductGroups { get; set; }

    }
}