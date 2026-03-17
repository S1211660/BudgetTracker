namespace BudgetTracker.Core.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}