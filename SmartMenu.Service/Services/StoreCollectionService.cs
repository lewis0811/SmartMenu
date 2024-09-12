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
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId).FirstOrDefault()
            ?? throw new Exception("Store Collection not found or deleted");

            _unitOfWork.StoreCollectionRepository.Remove(data);
            _unitOfWork.Save();
        }

        public IEnumerable<StoreCollection> GetAll(int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreCollectionRepository.EnableQuery()
                .Include(c => c.Collection!)
                    .ThenInclude(c => c.ProductGroups!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.ProductGroupItems!.Where(c => !c.IsDeleted))
                            .ThenInclude(c => c.Product!)
                                .ThenInclude(c => c.ProductSizePrices!.Where(c => !c.IsDeleted))
                .Where(c => !c.Collection!.IsDeleted);

            if (storeId != null)
            {
                AddCollectionForStore(storeId);

            }

            var result = DataQuery(data, storeCollectionId, storeId, collectionId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<StoreCollection>();
        }



        public StoreCollection Update(int storeCollectionId, StoreCollectionCreateDTO storeCollectionUpdateDTO)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId).FirstOrDefault()
                ?? throw new Exception("Store Collection not found or deleted");

            _mapper.Map(storeCollectionUpdateDTO, data);
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        public void DeleteV2(int storeCollectionId)
        {
            var data = _unitOfWork.StoreCollectionRepository.Find(c => c.StoreCollectionId == storeCollectionId).FirstOrDefault()
                ?? throw new Exception("Store Collection not found or deleted");

            data.IsDeleted = !data.IsDeleted;
            _unitOfWork.StoreCollectionRepository.Update(data);
            _unitOfWork.Save();
        }
        private void AddCollectionForStore(int? storeId)
        {
            var store = _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.StoreCollections.Where(c => !c.IsDeleted))
                .FirstOrDefault(c => c.StoreId == storeId && !c.IsDeleted)
                ?? throw new Exception("Store not found or deleted");

            var collections = _unitOfWork.CollectionRepository.EnableQuery()
                .Where(c => c.BrandId == store.BrandId && !c.IsDeleted)
                .ToList();

            if (store.StoreCollections.Count < collections.Count)
            {
                foreach (Collection collection in collections)
                {
                    if (store.StoreCollections != null && store.StoreCollections!.Any(c => c.CollectionId == collection.CollectionId)) continue;

                    StoreCollection storeCollection = new()
                    {
                        CollectionId = collection.CollectionId,
                        StoreId = store.StoreId,
                    };
                    _unitOfWork.StoreCollectionRepository.Add(storeCollection);
                    _unitOfWork.Save();

                }
            }
        }
        private IEnumerable<StoreCollection> DataQuery(IQueryable<StoreCollection> data, int? storeCollectionId, int? storeId, int? collectionId, string? searchString, int pageNumber, int pageSize)
        {
            //data = data.Where(c => c.IsDeleted == false);
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

            if (searchString != null)
            {
                data = data
                    .Where(c => c.Collection!.CollectionName.Contains(searchString)
                    || c.Collection.CollectionDescription!.Contains(searchString));
            }

            return PaginatedList<StoreCollection>.Create(data, pageNumber, pageSize);
        }
    }
}