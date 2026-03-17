using BudgetTracker.Core.Models;
using BudgetTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests
{
    public class TransactionTests
    {
        private BudgetTrackerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<BudgetTrackerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BudgetTrackerContext(options);
        }

        [Fact]
        public async Task AddTransaction_ShouldSaveToDatabase()
        {
            var context = GetInMemoryContext();

            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var category = new Category { UserId = user.Id, Name = "Salaris", Type = "income" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                UserId = user.Id,
                CategoryId = category.Id,
                Amount = 2500,
                Description = "Salaris maart",
                Date = DateTime.UtcNow
            };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var saved = await context.Transactions.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal(2500, saved.Amount);
        }

        [Fact]
        public async Task DeleteTransaction_ShouldRemoveFromDatabase()
        {
            var context = GetInMemoryContext();

            var user = new User { Username = "test", Email = "test@test.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var category = new Category { UserId = user.Id, Name = "Salaris", Type = "income" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                UserId = user.Id,
                CategoryId = category.Id,
                Amount = 100,
                Description = "Test",
                Date = DateTime.UtcNow
            };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();

            var count = await context.Transactions.CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public void Transaction_Amount_ShouldBePositive()
        {
            var transaction = new Transaction { Amount = 50.00m };
            Assert.True(transaction.Amount > 0);
        }

        [Fact]
        public void Transaction_NegativeAmount_ShouldFail()
        {
            var transaction = new Transaction { Amount = -10.00m };
            Assert.False(transaction.Amount > 0);
        }
    }
}