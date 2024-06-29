using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface IProductGroupService
    {
        IEnumerable<ProductGroup> GetAll(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        IEnumerable<ProductGroup> GetProductGroupWithGroupItem(int? productGroupId, int? menuId, int? collectionId, string? searchString, int pageNumber, int pageSize);

        ProductGroup Add(ProductGroupCreateDTO productGroupCreateDTO);
        ProductGroup Update(int productGroupId, ProductGroupUpdateDTO productGroupUpdateDTO);
        void Delete(int productGroupId);
    }
}
