using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class SubscriptionCreateDTO
    {
        [RegularExpression(@"^[a-zA-Z0-9\s_áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ]+$", ErrorMessage = "Invalid Name (Doesn't allow special characters)")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(50000, int.MaxValue, ErrorMessage = "Min price is 50,000")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Min duration is 1 Day")]
        public int DayDuration { get; set; }
    }
}