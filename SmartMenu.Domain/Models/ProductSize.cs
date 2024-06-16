namespace SmartMenu.Domain.Models
{
    public class ProductSize : BaseModel
    {
        public int ProductSizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
    }
}