using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ICollectionService
    {
        IEnumerable<Collection> GetAll(int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Collection> GetCollectionWithProductGroup(int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize);
        Collection Add(CollectionCreateDTO collectionCreateDTO);
        Collection Update(int collectionId, CollectionUpdateDTO collectionUpdateDTO);
        void Delete(int collectionId);

    }
}
