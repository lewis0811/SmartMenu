using Microsoft.AspNetCore.Mvc;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult Get(int? transactionId, int? deviceSubscriptionId, string? searchString, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var transactions = _transactionService.GetAll(transactionId, deviceSubscriptionId, searchString, pageNumber, pageSize);

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(TransactionCreateDTO transactionCreateDTO)
        {
            try
            {
                var data = await _transactionService.AddTransaction(transactionCreateDTO);

                return CreatedAtAction(nameof(Get), data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update(int transactionId, TransactionUpdateDTO transactionUpdateDTO)
        {
            try
            {
                var data = await _transactionService.Update(transactionId, transactionUpdateDTO);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{transactionId}")]
        public IActionResult Delete(int transactionId)
        {
            try
            {
                _transactionService.Delete(transactionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}