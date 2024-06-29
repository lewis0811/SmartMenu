using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CollectionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Collection Add(CollectionCreateDTO collectionCreateDTO)
        {
            var data = _mapper.Map<Collection>(collectionCreateDTO);

            _unitOfWork.CollectionRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int collectionId)
        {
            var data = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == collectionId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Collection not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.CollectionRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Collection> GetAll(int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.CollectionRepository.EnableQuery();
            var result = DataQuery(data, collectionId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Collection>();
        }

        public IEnumerable<Collection> GetCollectionWithProductGroup(int? collectionId, int? brandId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            var data = _unitOfWork.CollectionRepository.EnableQuery();
            data = data.Include(c => c.ProductGroups);

            var result = DataQuery(data, collectionId, brandId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Collection>();
        }

        public Collection Update(int collectionId, CollectionUpdateDTO collectionUpdateDTO)
        {
            var data = _unitOfWork.CollectionRepository.Find(c => c.CollectionId == collectionId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Collection not found or deleted");

            _mapper.Map(collectionUpdateDTO, data);
            _unitOfWork.CollectionRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<Collection> DataQuery(IQueryable<Collection> data, int? collectionId, int? brandId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (collectionId != null)
            {
                data = data
                    .Where(c => c.CollectionId == collectionId);
            }

            if (brandId != null)
            {
                data = data
                    .Where(c => c.BrandId == brandId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.CollectionName.Contains(searchString)
                    || c.CollectionDescription!.Contains(searchString));
            }

            return PaginatedList<Collection>.Create(data, pageNumber, pageSize);
        }
    }
}
