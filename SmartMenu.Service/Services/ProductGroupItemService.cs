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
    public class ProductGroupItemService : IProductGroupItemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductGroupItemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public ProductGroupItem Add(ProductGroupItemCreateDTO productGroupItemCreateDTO)
        {
            var data = _mapper.Map<ProductGroupItem>(productGroupItemCreateDTO);

            _unitOfWork.ProductGroupItemRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int productGroupItemId)
        {
            var data = _unitOfWork.ProductGroupItemRepository.Find(c => c.ProductGroupItemId == productGroupItemId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Group item not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductGroupItemRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductGroupItem> GetAll(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductGroupItemRepository.EnableQuery();
            var result = DataQuery(data, productGroupItemId, productGroupId, productId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<ProductGroupItem>();
        }

        public ProductGroupItem Update(int productGroupItemId, ProductGroupItemCreateDTO productGroupItemCreateDTO)
        {
            var data = _unitOfWork.ProductGroupItemRepository.Find(c => c.ProductGroupItemId == productGroupItemId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Group item not found or deleted");

            _mapper.Map(productGroupItemCreateDTO, data);
            _unitOfWork.ProductGroupItemRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<ProductGroupItem> DataQuery(IQueryable<ProductGroupItem> data, int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productGroupItemId != null)
            {
                data = data
                    .Where(c => c.ProductGroupId == productGroupItemId);
            }

            if (productGroupId != null)
            {
                data = data
                    .Where(c => c.ProductGroupId == productGroupId);
            }
            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductId == productId);
            }

            if (searchString != null)
            {
                data = data
                    .Where(c => c.Product!.ProductName.Contains(searchString)
                    || c.Product.ProductSizePrices!.Any(d => d.Price.ToString().Contains(searchString))
                    || c.Product.ProductSizePrices!.Any(d => d.ProductSizeType.ToString().Contains(searchString))
                    );
            }

            return PaginatedList<ProductGroupItem>.Create(data, pageNumber, pageSize);
        }
    }
}
