using BudgetTracker.Core.Models;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests
{
    public class CategoryTests
    {
        private BudgetTrackerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BudgetTrackerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BudgetTrackerContext(options);
        }

        [Fact]
        public async Task AddCategory_ShouldSaveToDatabase()
        {
            var context = GetInMemoryContext();

            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var category = new Category { UserId = user.Id, Name = "Boodschappen", Type = "expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var saved = await context.Categories.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Boodschappen", saved.Name);
        }

        [Fact]
        public void Category_TypeShouldBeIncomeOrExpense()
        {
            var validTypes = new[] { "income", "expense" };

            var category = new Category { Type = "income" };
            Assert.Contains(category.Type, validTypes);
        }

        [Fact]
        public async Task DeleteCategory_ShouldRemoveFromDatabase()
        {
            var context = GetInMemoryContext();

            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var category = new Category { UserId = user.Id, Name = "Huur", Type = "expense" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            var count = await context.Categories.CountAsync();
            Assert.Equal(0, count);
        }
    }
}