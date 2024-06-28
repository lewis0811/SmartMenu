using SmartMenu.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductSizePriceUpdateDTO
    {
        public ProductSizeType ProductSizeType{ get; set; }
        public double Price { get; set; }
    }
}
