using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMenu.Service.Interfaces;
using SmartMenu.DAO;
using AutoMapper;
using SmartMenu.Domain.Repository;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace SmartMenu.Service.Services
{
    public class StoreCollectionService : IStoreCollectionService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreCollectionService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        private IEnumerable<StoreCollection> DataQuery(IQueryable<StoreCollection> data, int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeCollectionId != null)
            {
                data = data
                    .Where(c => c.StoreCollectionId == storeCollectionId);
            }

            if (storeId != null)
            {
                data = data
                    .Where(c => c.StoreId == storeId);
            }
            if (collectionId != null)
            {
                data = data
                    .Where(c => c.CollectionId == collectionId);
            }


            return PaginatedList<StoreCollection>.Create(data, pageNumber, pageSize);
        }
        public StoreCollection Add(StoreCollectionCreateDTO storeCollectionCreateDTO)
        {
            var st = _unitOfWork.StoreRepository
                .Find(c => c.StoreId == storeCollectionCreateDTO.StoreID && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            var cl = _unitOfWork.CollectionRepository
                .Find(c => c.CollectionId == storeCollectionCreateDTO.CollectionID && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Collection not found or deleted");

            var data = _mapper.Map<StoreCollection>(storeCollectionCreateDTO);

            _unitOfWork.StoreCollectionRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int storeCollectionId)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Box not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<StoreCollection> GetAll(int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreCollectionRepository.EnableQuery()
                .Include(c => c.Collection)
                .Where(c => !c.Collection!.IsDeleted);

            var result = DataQuery(data, storeCollectionId, storeId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<StoreCollection>();
        }


        public StoreCollection Update(int storeCollectionId, StoreCollectionCreateDTO storeCollectionUpdateDTO)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store collection not found or deleted");

            _mapper.Map(storeCollectionUpdateDTO, data);
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
    }
}
