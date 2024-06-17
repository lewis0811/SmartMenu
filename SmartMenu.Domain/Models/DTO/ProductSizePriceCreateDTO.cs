namespace SmartMenu.Domain.Models.DTO
{
    public class ProductSizePriceCreateDTO
    {
        public int ProductId { get; set; }
        public int ProductSizeId { get; set; }
        public double Price { get; set; }
    }
}