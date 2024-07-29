using AutoMapper;
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
    public class ProductSizePriceService : IProductSizePriceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizePriceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public ProductSizePrice Add(ProductSizePriceCreateDTO productSizePriceCreateDTO)
        {
            var data = _mapper.Map<ProductSizePrice>(productSizePriceCreateDTO);

            _unitOfWork.ProductSizePriceRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int productSizePriceId)
        {
            var data = _unitOfWork.ProductSizePriceRepository.Find(c => c.ProductSizePriceId == productSizePriceId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Size price not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductSizePriceRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductSizePrice> GetAll(int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductSizePriceRepository.EnableQuery();
            var result = DataQuery(data, productSizePriceId, productId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<ProductSizePrice>();
        }

        public ProductSizePrice Update(int productSizePriceId, ProductSizePriceUpdateDTO productSizePriceUpdateDTO)
        {
            var data = _unitOfWork.ProductSizePriceRepository.Find(c => c.ProductSizePriceId == productSizePriceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Size price not found or deleted");

            _mapper.Map(productSizePriceUpdateDTO, data);
            _unitOfWork.ProductSizePriceRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<ProductSizePrice> DataQuery(IQueryable<ProductSizePrice> data, int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);

            if (productSizePriceId != null)
            {
                data = data.Where(c => c.ProductSizePriceId == productSizePriceId);
            }

            if (productId != null)
            {
                data = data.Where(c => c.ProductId == productId);
            }

            if (searchString != null)
            {
                if (double.TryParse(searchString, out double result))
                {
                    data = data.Where(c => c.Price == result);
                }

                if (Enum.TryParse(typeof(ProductSizeType), searchString, out var result2))
                {
                    data = data.Where(c => c.ProductSizeType == (ProductSizeType)result2!);
                }

            }

            return PaginatedList<ProductSizePrice>.Create(data, pageNumber, pageSize);
        }
    }
}
