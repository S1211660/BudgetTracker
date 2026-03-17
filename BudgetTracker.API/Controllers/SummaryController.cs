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
    public class SummaryController : ControllerBase
    {
        private readonly BudgetTrackerContext _context;

        public SummaryController(BudgetTrackerContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/summary?month=3&year=2026
        [HttpGet]
        public async Task<IActionResult> GetSummary(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t =>
                    t.UserId == GetUserId() &&
                    t.Date.Month == month &&
                    t.Date.Year == year)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.Category.Type == "income")
                .Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.Category.Type == "expense")
                .Sum(t => t.Amount);

            var byCategory = transactions
                .GroupBy(t => new { t.Category.Name, t.Category.Type })
                .Select(g => new
                {
                    Category = g.Key.Name,
                    Type = g.Key.Type,
                    Total = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return Ok(new
            {
                Month = month,
                Year = year,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                ByCategory = byCategory
            });
        }
    }
}