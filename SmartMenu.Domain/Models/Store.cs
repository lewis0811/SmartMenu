using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Store : BaseModel
    {
        public int StoreId { get; set; }
        public int BrandId { get; set; }

        [Required]
        public string StoreCode { get; set; } = string.Empty;

        [Required]
        public string StoreName { get; set; } = string.Empty;

        [Required]
        public string StoreLocation { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string StoreContactEmail { get; set; } = string.Empty;

        [Phone]
        [Required]
        public string StoreContactNumber { get; set; } = string.Empty;

        public bool StoreStatus { get; set; }

        [ForeignKey(nameof(BrandId))]
        public Brand? Brand { get; set; } //

        public ICollection<BrandStaff> Staffs { get; set; } = new List<BrandStaff>();
        public ICollection<StoreMenu>? StoreMenus { get; set; } = new List<StoreMenu>();
        public ICollection<StoreCollection>? StoreCollections { get; set; } = new List<StoreCollection>();
        public ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
    }
}