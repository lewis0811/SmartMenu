using AutoMapper;
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
    public class StoreProductService : IStoreProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreProductService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public StoreProduct Add(StoreProductCreateDTO storeProductCreateDTO)
        {
            var st = _unitOfWork.StoreRepository
                .Find(c => c.StoreId == storeProductCreateDTO.StoreId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            var pr = _unitOfWork.ProductRepository
                .Find(c => c.ProductId == storeProductCreateDTO.ProductId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Product not found or deleted");

            var data = _mapper.Map<StoreProduct>(storeProductCreateDTO);

            _unitOfWork.StoreProductRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int storeProductId)
        {
            var data = _unitOfWork.StoreProductRepository.Find(c => c.StoreProductId == storeProductId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Store product not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.StoreProductRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<StoreProduct> GetAll(int? storeProductId, int? storeId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreProductRepository.EnableQuery();
            var result = DataQuery(data, storeProductId, storeId, productId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<StoreProduct>();
        }

        public StoreProduct Update(int storeProductId, StoreProductUpdateDTO storeProductUpdateDTO)
        {
            var data = _unitOfWork.StoreProductRepository.Find(c => c.StoreProductId == storeProductId && c.IsDeleted == false).FirstOrDefault()
                 ?? throw new Exception("Store product not found or deleted");

            _mapper.Map(storeProductUpdateDTO, data);
            _unitOfWork.StoreProductRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<StoreProduct> DataQuery(IQueryable<StoreProduct> data, int? storeProductId, int? storeId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (storeProductId != null)
            {
                data = data
                    .Where(c => c.StoreProductId == storeProductId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.Product!.ProductName.Contains(searchString));
            }

            return PaginatedList<StoreProduct>.Create(data, pageNumber, pageSize);
        }
    }
}
