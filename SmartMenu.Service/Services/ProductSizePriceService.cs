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
    public class ProductSizePriceService : IProductSizePriceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizePriceService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ProductSizePrice> AddAsync(ProductSizePriceCreateDTO productSizePriceCreateDTO)
        {

            var existProductSizePrice = await _unitOfWork.ProductSizePriceRepository
                .FindObjectAsync(c => c.ProductId == productSizePriceCreateDTO.ProductId && c.IsDeleted == false);

            if (existProductSizePrice != null)
            {
                if (existProductSizePrice.ProductSizeType != ProductSizeType.Normal && productSizePriceCreateDTO.ProductSizeType == ProductSizeType.Normal) throw new Exception("This product already have price based on size, it can't have normal price");
                if (existProductSizePrice.ProductSizeType == ProductSizeType.Normal && productSizePriceCreateDTO.ProductSizeType != ProductSizeType.Normal) throw new Exception("This product already have a normal price, it can't have size based price");

                var existProductSizeType = await _unitOfWork.ProductSizePriceRepository
                    .FindObjectAsync(c => c.ProductId == productSizePriceCreateDTO.ProductId && c.ProductSizeType == productSizePriceCreateDTO.ProductSizeType && !c.IsDeleted);

                if (existProductSizeType != null)
                {
                    throw new Exception($"This product already have a price size {productSizePriceCreateDTO.ProductSizeType}");
                }
            }


            var data = _mapper.Map<ProductSizePrice>(productSizePriceCreateDTO);
            await CheckValidPrice(data);

            _unitOfWork.ProductSizePriceRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }



        public void Delete(int productSizePriceId)
        {
            var data = _unitOfWork.ProductSizePriceRepository.Find(c => c.ProductSizePriceId == productSizePriceId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Size price not found or deleted");


            _unitOfWork.ProductSizePriceRepository.Remove(data);
            _unitOfWork.Save();
        }

        public IEnumerable<ProductSizePrice> GetAll(int? productSizePriceId, int? productId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductSizePriceRepository.EnableQuery();
            var result = DataQuery(data, productSizePriceId, productId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<ProductSizePrice>();
        }

        public async Task<ProductSizePrice> Update(int productSizePriceId, ProductSizePriceUpdateDTO productSizePriceUpdateDTO)
        {
            var data = _unitOfWork.ProductSizePriceRepository.Find(c => c.ProductSizePriceId == productSizePriceId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Size price not found or deleted");

            _mapper.Map(productSizePriceUpdateDTO, data);
            await CheckValidUpdatePrice(data);

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
        private async Task CheckValidPrice(ProductSizePrice data)
        {
            var totalPriceOfProduct = await _unitOfWork.ProductSizePriceRepository
                .FindListAsync(c => c.ProductId == data.ProductId && !c.IsDeleted);

            if (totalPriceOfProduct.Any())
            {
                switch (data.ProductSizeType)
                {
                    case ProductSizeType.S:
                        var anotherSizeType = totalPriceOfProduct.Where(c => c.Price <= data.Price).FirstOrDefault();
                        if (anotherSizeType != null)
                        {
                            throw new Exception($"Size S price can't have price lower than other price");
                        }
                        break;

                    case ProductSizeType.M:
                        var existSizeS = totalPriceOfProduct.Where(c => c.ProductSizeType == ProductSizeType.S && !c.IsDeleted).FirstOrDefault();
                        var existSizeL = totalPriceOfProduct.Where(c => c.ProductSizeType == ProductSizeType.L && !c.IsDeleted).FirstOrDefault();

                        if (existSizeS != null)
                        {
                            if (data.Price < existSizeS.Price) throw new Exception($"Size M price must higher than Size {existSizeS.ProductSizeType} price");
                        }
                        if (existSizeL != null)
                        {
                            if (data.Price > existSizeL.Price) throw new Exception($"Size M price must lower than than Size {existSizeL.ProductSizeType} price");
                        }
                        break;

                    case ProductSizeType.L:
                        var anotherSizeType2 = totalPriceOfProduct.Where(c => c.Price >= data.Price).FirstOrDefault();
                        if (anotherSizeType2 != null)
                        {
                            throw new Exception($"Size L price can't have lower price than other price");
                        }
                        break;
                }
            }
        }
        private async Task CheckValidUpdatePrice(ProductSizePrice data)
        {
            var totalPriceOfProduct = await _unitOfWork.ProductSizePriceRepository.EnableQuery()
                .Where(c => c.ProductId == data.ProductId && c.ProductSizeType != data.ProductSizeType && !c.IsDeleted)
                .ToListAsync();

            if (totalPriceOfProduct.Any())
            {
                switch (data.ProductSizeType)
                {
                    case ProductSizeType.S:
                        var anotherSizeType = totalPriceOfProduct.Where(c => c.Price <= data.Price).FirstOrDefault();
                        if (anotherSizeType != null)
                        {
                            throw new Exception($"Size S price can't have price lower than other price");
                        }
                        break;

                    case ProductSizeType.M:
                        var existSizeS = totalPriceOfProduct.Where(c => c.ProductSizeType == ProductSizeType.S && !c.IsDeleted).FirstOrDefault();
                        var existSizeL = totalPriceOfProduct.Where(c => c.ProductSizeType == ProductSizeType.L && !c.IsDeleted).FirstOrDefault();

                        if (existSizeS != null)
                        {
                            if (data.Price < existSizeS.Price) throw new Exception($"Size M price must higher than Size {existSizeS.ProductSizeType} price");
                        }
                        if (existSizeL != null)
                        {
                            if (data.Price > existSizeL.Price) throw new Exception($"Size M price must lower than than Size {existSizeL.ProductSizeType} price");
                        }
                        break;

                    case ProductSizeType.L:
                        var anotherSizeType2 = totalPriceOfProduct.Where(c => c.Price >= data.Price).FirstOrDefault();
                        if (anotherSizeType2 != null)
                        {
                            throw new Exception($"Size L price can't have lower price than other price");
                        }
                        break;
                }
            }
        }
    }
}
