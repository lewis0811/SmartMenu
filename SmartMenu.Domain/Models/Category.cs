namespace SmartMenu.Domain.Models
{
    public class Category : BaseModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
