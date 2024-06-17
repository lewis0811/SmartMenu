using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductSizePriceUpdateDTO
    {
        public int ProductSizeId { get; set; }
        public double Price { get; set; }
    }
}
