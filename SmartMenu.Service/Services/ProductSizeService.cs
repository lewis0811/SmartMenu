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
    public class ProductSizeService : IProductSizeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public ProductSize Add(ProductSizeCreateDTO productSizeCreateDTO)
        {
            var data = _mapper.Map<ProductSize>(productSizeCreateDTO);

            _unitOfWork.ProductSizeRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int productSizeId)
        {
            var data = _unitOfWork.ProductSizeRepository.Find(c => c.ProductSizeId == productSizeId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Size not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductSizeRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductSize> GetAll(int? productSizeId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductSizeRepository.EnableQuery();
            var result = DataQuery(data, productSizeId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<ProductSize>();
        }

        public ProductSize Update(int productSizeId, ProductSizeCreateDTO productSizeCreateDTO)
        {
            var data = _unitOfWork.ProductSizeRepository.Find(c => c.ProductSizeId == productSizeId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Size not found or deleted");

            _mapper.Map(productSizeCreateDTO, data);
            _unitOfWork.ProductSizeRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<ProductSize> DataQuery(IQueryable<ProductSize> data, int? productSizeId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            {
                data = data
                   .Where(c => c.ProductSizeId == productSizeId);
            }
            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                     .Where(c => c.SizeName.Contains(searchString));
            }
            return PaginatedList<ProductSize>.Create(data, pageNumber, pageSize);
        }
    }
}
