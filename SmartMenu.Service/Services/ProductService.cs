﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;
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
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Product Add(ProductCreateDTO productCreateDTO)
        {
            var data = _mapper.Map<Product>(productCreateDTO);

            _unitOfWork.ProductRepository.Add(data);
            _unitOfWork.Save();

            var brand = _unitOfWork.BrandRepository.EnableQuery()
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.Categories!.Any(c => c.CategoryId == data.CategoryId && !c.IsDeleted) && !b.IsDeleted)
                ?? throw new Exception("Brand not found or deleted");

            var storeOfBrand = _unitOfWork.StoreRepository.EnableQuery()
                .Include(c => c.StoreProducts)
                    .ThenInclude(c => c.Product)
                .Where(c => c.BrandId == brand.BrandId).ToList();

            if (storeOfBrand.Count != 0)
            {
                foreach (var store in storeOfBrand)
                {
                    if (!store.StoreProducts.Any(c => c.ProductId == data.ProductId))
                    {
                        var storeProduct = new StoreProduct
                        {
                            StoreId = store.StoreId,
                            ProductId = data.ProductId,
                            //IconEnable = false,
                            IsAvailable = true,
                        };
                        _unitOfWork.StoreProductRepository.Add(storeProduct);
                        _unitOfWork.Save();
                    }
                }
            }

            return data;
        }

        public void Delete(int productId)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Product not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();

            DeleteProductInProductGroup(productId);
            UpdateDisplayIfExist(productId);
        }

        public IEnumerable<Product> GetAll(int? productId, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductRepository.EnableQuery()
                .Include(c => c.ProductSizePrices!.Where(d => d.IsDeleted == false));
            var result = DataQuery(data, productId, categoryId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Product>();
        }

        public IEnumerable<Product> GetByBrand(int brandId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BrandRepository.EnableQuery()
                .Include(c => c.Categories!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Products!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.ProductSizePrices!.Where(c => !c.IsDeleted))
                .Where(c => c.BrandId == brandId && !c.IsDeleted)
                .SelectMany(c => c.Categories!).SelectMany(c => c.Products!);

            //var data = _unitOfWork.ProductRepository.EnableQuery()
            //    .Include(c => c.ProductSizePrices!.Where(d => d.IsDeleted == false));

            var result = DataQuery(data, null, null, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Product>();
        }

        public IEnumerable<Product> GetProductByMenuOrCollection(int? menuId, int? collectionId)
        {
            List<Product> menu = new();
            List<Product> collection = new();

            if (menuId != null || menuId != 0)
            {
                menu = _unitOfWork.MenuRepository.EnableQuery()
                .Include(c => c.ProductGroups!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.ProductGroupItems!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Product!)
                            .ThenInclude(c => c.ProductSizePrices)
                .Where(c => c.MenuId == menuId)
                    .SelectMany(c => c.ProductGroups!)
                        .SelectMany(c => c.ProductGroupItems!)
                            .Select(c => c.Product)
                .ToList()!;
            }

            if (collectionId != null || collectionId != 0)
            {
                collection = _unitOfWork.CollectionRepository.EnableQuery()
                    .Include(c => c.ProductGroups!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.ProductGroupItems!.Where(c => !c.IsDeleted))
                            .ThenInclude(c => c.Product!)
                                .ThenInclude(c => c.ProductSizePrices)
                    .Where(c => c.CollectionId == collectionId)
                        .SelectMany(c => c.ProductGroups!)
                            .SelectMany(c => c.ProductGroupItems!)
                                .Select(c => c.Product)
                    .ToList()!;
            }

            return menu.Count > 0
                ? menu
                : collection;
        }

        public IEnumerable<Product> GetProductByCollection(int collectionId)
        {
            var collection = _unitOfWork.CollectionRepository.EnableQuery()
                .Include(c => c.ProductGroups!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.ProductGroupItems!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Product!)
                            .ThenInclude(c => c.ProductSizePrices)
                .Where(c => c.CollectionId == collectionId)
                    .SelectMany(c => c.ProductGroups!)
                        .SelectMany(c => c.ProductGroupItems!)
                            .Select(c => c.Product)
                .ToList();

            return collection!;
        }

        public Product Update(int productId, ProductUpdateDTO productUpdateDTO)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Product not found or deleted");

            _mapper.Map(productUpdateDTO, data);
            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();

            var brand = _unitOfWork.BrandRepository
                .EnableQuery()
                .Include(c => c.Categories!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Products!.Where(c => !c.IsDeleted && c.ProductId == data.ProductId))
                .FirstOrDefault() ?? throw new Exception("Brand not found or is deleted");

            var displays = _unitOfWork.BrandRepository
                .EnableQuery()
                .Include(c => c.Categories!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Products!.Where(c => !c.IsDeleted && c.ProductId == data.ProductId))
                .Include(c => c.Stores!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.StoreDevices!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Displays!.Where(c => !c.IsDeleted))
                .SelectMany(c => c.Stores!.Where(c => c.BrandId == brand.BrandId && !c.IsDeleted)).SelectMany(c => c.StoreDevices.Where(c => !c.IsDeleted)).SelectMany(c => c.Displays!.Where(c => !c.IsDeleted))
                .ToList();

            foreach (var display in displays)
            {
                display.IsChanged = true;
                _unitOfWork.DisplayRepository.Update(display);
                _unitOfWork.Save();
            }

            return data;
        }

        private IEnumerable<Product> DataQuery(IQueryable<Product> data, int? productId, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductId == productId);
            }

            if (categoryId != null)
            {
                data = data
                    .Where(c => c.CategoryId == categoryId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                if (Enum.TryParse(typeof(ProductSizeType), searchString, out var result))
                {
                    data = data
                        .Where(c => c.ProductSizePrices!.Any(d => d.ProductSizeType.Equals(result)));
                }

                if (double.TryParse(searchString, out double resuslt))
                {
                    data = data.Where(c => c.ProductSizePrices!.Any(d => d.Price.Equals(resuslt)));
                }

                data = data
                    .Where(c => c.ProductName.Contains(searchString)
                    || c.ProductDescription!.Contains(searchString));
            }

            return PaginatedList<Product>.Create(data, pageNumber, pageSize);
        }

        private void DeleteProductInProductGroup(int productId)
        {
            var productGroupHoldingProduct = _unitOfWork.ProductGroupRepository.EnableQuery()
                .Include(c => c.ProductGroupItems!.Where(c => c.ProductId == productId && !c.IsDeleted))
                .ToList();

            if (productGroupHoldingProduct.Count > 0)
            {
                foreach (var productGroup in productGroupHoldingProduct)
                {
                    foreach (var product in productGroup.ProductGroupItems!)
                    {
                        var data2 = _unitOfWork.ProductGroupItemRepository
                            .Find(c => c.ProductId.Equals(product.ProductId))
                            .FirstOrDefault();

                        if (data2 != null)
                        {
                            _unitOfWork.ProductGroupItemRepository.Remove(data2);
                            _unitOfWork.Save();
                        }
                    }
                }
            }
        }

        private void UpdateDisplayIfExist(int productId)
        {
            var displays = _unitOfWork.BrandRepository.EnableQuery()
                .Include(c => c.Categories!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Products!.Where(c => c.ProductId == productId && !c.IsDeleted))
                .Include(c => c.Stores!.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.StoreDevices!.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Displays!.Where(c => !c.IsDeleted))
                .SelectMany(c => c.Stores!).SelectMany(c => c.StoreDevices!).SelectMany(c => c.Displays!)
                .ToList();

            if (displays.Count > 0)
            {
                foreach (var display in displays)
                {
                    display.IsChanged = true;
                    _unitOfWork.DisplayRepository.Update(display);
                    _unitOfWork.Save();
                }
            }
        }
    }
}