using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models
{
    public class Category : BaseModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
