//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using SmartMenu.Domain.Models;
//using SmartMenu.Domain.Models.DTO;
//using SmartMenu.Domain.Repository;
//using SmartMenu.Service.Interfaces;
//using SmartMenu.Service.Services;

//namespace SmartMenu.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductSizesController : ControllerBase
//    {
//        private readonly IMapper _mapper;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IProductSizeService _productSizeService;

//        public ProductSizesController(IMapper mapper, IUnitOfWork unitOfWork, IProductSizeService productSizeService)
//        {
//            _mapper = mapper;
//            _unitOfWork = unitOfWork;
//            _productSizeService = productSizeService;
//        }

//        [HttpGet]
//        public IActionResult Get(int? productSizeId, string? searchString, int pageNumber = 1, int pageSize = 10)
//        {
//            try
//            {
//                var data = _productSizeService.GetAll(productSizeId, searchString, pageNumber, pageSize);
//                return Ok(data);
//            }
//            catch (Exception ex)
//            {
//
//                

//            };
//        }

//        [HttpPost]
//        public IActionResult Add(ProductSizeCreateDTO productSizeCreateDTO)
//        {
//            try
//            {
//                var data = _productSizeService.Add(productSizeCreateDTO);
//                return CreatedAtAction(nameof(Get), data);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new {error = ex.Message });
//            }
//        }

//        [HttpPut("{productSizeId}")]
//           public ActionResult Update(int productSizeId, ProductSizeCreateDTO productSizeCreateDTO)
//        {
//            try
//            {
//                var data = _productSizeService.Update(productSizeId, productSizeCreateDTO);
//                return Ok(data);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new {error = ex.Message });
//            }
//        }
//        [HttpDelete("{productSizeId}")]
//        public IActionResult Delete(int productSizeId)
//        {
//            try
//            {
//                _productSizeService.Delete(productSizeId);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new {error = ex.Message });
//            }
//        }
//    }

//}