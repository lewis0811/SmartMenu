using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Category : BaseModel
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [ForeignKey(nameof(BrandId))]
        public Brand? Brand { get; set; } //

        public ICollection<Product>? Products { get; set; }
    }
}