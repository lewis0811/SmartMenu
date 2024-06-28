namespace SmartMenu.Domain.Models.DTO
{
    public class CategoryCreateDTO
    {
        public int BrandId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
