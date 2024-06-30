using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IProductGroupItemService
    {
        IEnumerable<ProductGroupItem> GetAll(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber, int pageSize);
        ProductGroupItem Add(ProductGroupItemCreateDTO productGroupItemCreateDTO);
        ProductGroupItem Update(int productGroupItemId, ProductGroupItemCreateDTO productGroupItemCreateDTO);
        void Delete(int productGroupItemId);
    }
}
