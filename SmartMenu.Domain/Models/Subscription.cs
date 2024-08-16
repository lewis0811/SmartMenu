using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models
{
    public class Subscription : BaseModel
    {
        public int SubscriptionId { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s_áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđ]+$", ErrorMessage = "Invalid Name (Doesn't allow special characters)")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Range(50000, int.MaxValue, ErrorMessage = "Min price is 50,000")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Min duration is 1 Day")]
        public int DayDuration { get; set; }

        public bool IsActive { get; set; } = true;
    }
}