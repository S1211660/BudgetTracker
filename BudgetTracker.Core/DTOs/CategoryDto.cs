namespace BudgetTracker.Core.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "income" of "expense"
    }
}