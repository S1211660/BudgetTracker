using BudgetTracker.Core.DTOs;
using BudgetTracker.Core.Models;
using BudgetTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly BudgetTrackerContext _context;

        public TransactionsController(BudgetTrackerContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions(
            [FromQuery] int? month,
            [FromQuery] int? year,
            [FromQuery] string? type)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == GetUserId());

            if (month.HasValue)
                query = query.Where(t => t.Date.Month == month.Value);

            if (year.HasValue)
                query = query.Where(t => t.Date.Year == year.Value);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(t => t.Category.Type == type);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date,
                    CategoryName = t.Category.Name,
                    CategoryType = t.Category.Type
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

            if (transaction == null) return NotFound();

            return Ok(new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                CategoryName = transaction.Category.Name,
                CategoryType = transaction.Category.Type
            });
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto dto)
        {
            // Controleer of categorie van deze user is
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == GetUserId());

            if (category == null)
                return BadRequest("Categorie niet gevonden.");

            if (dto.Amount <= 0)
                return BadRequest("Bedrag moet groter dan 0 zijn.");

            var transaction = new Transaction
            {
                UserId = GetUserId(),
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                CategoryName = category.Name,
                CategoryType = category.Type
            });
        }

        // PUT: api/transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, CreateTransactionDto dto)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

            if (transaction == null) return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == GetUserId());

            if (category == null)
                return BadRequest("Categorie niet gevonden.");

            if (dto.Amount <= 0)
                return BadRequest("Bedrag moet groter dan 0 zijn.");

            transaction.CategoryId = dto.CategoryId;
            transaction.Amount = dto.Amount;
            transaction.Description = dto.Description;
            transaction.Date = dto.Date;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

            if (transaction == null) return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}