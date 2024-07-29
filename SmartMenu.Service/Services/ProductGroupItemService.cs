using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
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
            var productGroup = _unitOfWork.ProductGroupRepository
                .Find(c => c.ProductGroupId == productGroupItemCreateDTO.ProductGroupId && c.IsDeleted == false)
                .FirstOrDefault() ?? throw new Exception($"Product group ID: {productGroupItemCreateDTO.ProductGroupId} not found or deleted");

            var product = _unitOfWork.ProductRepository
                .EnableQuery()
                .Include(c => c.ProductSizePrices)
                .Where(c => c.ProductId == productGroupItemCreateDTO.ProductId && c.IsDeleted == false)
                .FirstOrDefault() ?? throw new Exception($"Product ID: {productGroupItemCreateDTO.ProductId} not found or deleted");

            if (product.ProductSizePrices!.Count == 0) throw new Exception($"Product ID: {product.ProductId} haven't initialize price ");

            var productSizeType = product.ProductSizePrices.FirstOrDefault()!.ProductSizeType;

            if (productGroup.HaveNormalPrice && productSizeType  != Domain.Models.Enum.ProductSizeType.Normal)
            {
                throw new Exception($"This product ID: {product.ProductId} have a price based on size of product " +
                    $"\nSo it not match with product group ID: {productGroup.ProductGroupId} that is defined as having a normal price ");
            } 

            if (!productGroup.HaveNormalPrice && productSizeType == Domain.Models.Enum.ProductSizeType.Normal)
            {
                throw new Exception($"This product ID: {product.ProductId} have a normal price" +
                    $"\nSo it not match with product group ID: {productGroup.ProductGroupId} that is defined as having a price based on size of product");
            }

            var existProductInProductGroupItem = _unitOfWork.ProductGroupItemRepository
                .Find(c => c.ProductId == productGroupItemCreateDTO.ProductId && c.ProductGroupId == productGroupItemCreateDTO.ProductGroupId)
                .Any();
            if (existProductInProductGroupItem) throw new Exception($"Product ID: {productGroupItemCreateDTO.ProductId} already exist in the product group ID: {productGroupItemCreateDTO.ProductGroupId}");

            var data = _mapper.Map<ProductGroupItem>(productGroupItemCreateDTO);

            _unitOfWork.ProductGroupItemRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int productGroupItemId)
        {
            var data = _unitOfWork.ProductGroupItemRepository.Find(c => c.ProductGroupItemId == productGroupItemId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Group item not found or deleted");

            //data.IsDeleted = true;
            //_unitOfWork.ProductGroupItemRepository.Update(data);

            _unitOfWork.ProductGroupItemRepository.Remove(data);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductGroupItem> GetAll(int? productGroupItemId, int? productGroupId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductGroupItemRepository.EnableQuery()
                .Include(c => c.Product)
                    .ThenInclude(c => c!.ProductSizePrices);
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
                searchString = searchString.Trim();
                if(Enum.TryParse(typeof(ProductSizeType), searchString, out var sizeType)) {
                    data = data
                        .Where(c => c.Product!.ProductSizePrices!.Any(d => d.ProductSizeType.Equals((ProductSizeType)sizeType!))
                        );
                }

                if (double.TryParse(searchString, out double price))
                {
                    data = data
                        .Where(c =>
                            c.Product!.ProductSizePrices!.Any(d => d.Price.Equals(double.Parse(searchString)))
                            );
                }

                data = data.Where(c => c.Product!.ProductName.Contains(searchString));
            }

            return PaginatedList<ProductGroupItem>.Create(data, pageNumber, pageSize);
        }
    }
}
