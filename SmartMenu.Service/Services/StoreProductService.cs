using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

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

        public List<StoreProduct> AddV2(StoreProductCreateDTO_V2 storeProductCreateDTO)
        {
            var st = _unitOfWork.StoreRepository
                .Find(c => c.StoreId == storeProductCreateDTO.StoreId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            var category = _unitOfWork.CategoryRepository
                .EnableQuery()
                .Include(c => c.Products)
                .Where(c => c.CategoryId == storeProductCreateDTO.CategoryId && c.IsDeleted == false && c.Products!.Count() > 0)
                .FirstOrDefault() ?? throw new Exception("Category not found or deleted or there's no product in this category");

            var existedStoreProducts = _unitOfWork.StoreProductRepository
                .Find(c => c.StoreId == storeProductCreateDTO.StoreId && c.IsDeleted == false)
                .ToList();

            var storeProducts = new List<StoreProduct>();

            foreach (var product in category.Products!)
            {
                if (existedStoreProducts.Any(c => c.ProductId == product.ProductId)) continue;

                var sp = new StoreProduct
                {
                    StoreId = storeProductCreateDTO.StoreId,
                    ProductId = product.ProductId,
                    IsAvailable = true,
                    IsDeleted = false,
                };
                storeProducts.Add(sp);
            };

            if (storeProducts.Count == 0) throw new Exception($"Products in category ID: {storeProductCreateDTO.CategoryId} have been added before.");

            _unitOfWork.StoreProductRepository.AddRange(storeProducts);
            _unitOfWork.Save();

            return storeProducts;
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
            var data = _unitOfWork.StoreProductRepository.EnableQuery()
                .Include(c => c.Product).Where(c => !c.Product!.IsDeleted);

            var result = DataQuery(data, storeProductId, storeId, productId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<StoreProduct>();
        }

        public IEnumerable<StoreProduct> GetWithProductSizePrices(int? storeProductId, int? storeId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.StoreProductRepository.EnableQuery()
                .Where(c => !c.Product!.IsDeleted)
                .Include(c => c.Product!)
                    .ThenInclude(c => c.ProductSizePrices);
                
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

            if(storeId != null)
            {
                data = data
                    .Where(c => c.StoreId == storeId);
            }

            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductId == productId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();

                if (data.Any(c => c.Product!.ProductSizePrices != null))
                {
                    if (Enum.TryParse(typeof(ProductSizeType), searchString, out var result))
                    {
                        data = data
                            .Where(c => c.Product!.ProductSizePrices!.Any(d => d.ProductSizeType.Equals(result)));
                    }

                    if (double.TryParse(searchString, out double resuslt))
                    {
                        data = data
                            .Where(c => c.Product!.ProductSizePrices!.Any(d => d.Price.Equals(resuslt)));
                    }
                }

                data = data
                    .Where(c => c.Product!.ProductName.Contains(searchString)
                    || c.Product.ProductDescription!.Contains(searchString));
            }

            return PaginatedList<StoreProduct>.Create(data, pageNumber, pageSize);
        }
    }
}