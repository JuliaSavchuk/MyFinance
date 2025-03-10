using Microsoft.EntityFrameworkCore;
using MyFinance.Models;

namespace MyFinance.Services
{
    public class BudgetService
    {
        private readonly AppDbContext _context;

        public BudgetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Budget> GetBudgetByIdAsync(int budgetId)
        {
            try
            {
                return await _context.Budgets.FindAsync(budgetId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get budget", ex);
            }
        }

        public async Task<List<Budget>> GetBudgetsByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get budgets", ex);
            }
        }

        public async Task<Budget> CreateBudgetAsync(int userId, string name, decimal limit)
        {
            try
            {
                var budget = new Budget
                {
                    UserId = userId,
                    Name = name,
                    Limit = limit,
                    TotalIncome = 0,
                    TotalExpense = 0,
                    Balance = 0 // Явно встановлюємо початковий баланс
                };
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();
                return budget;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create budget", ex);
            }
        }

        public async Task UpdateBudgetAsync(Budget budget)
        {
            try
            {
                _context.Budgets.Update(budget);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update budget", ex);
            }
        }

        public async Task DeleteBudgetAsync(int budgetId)
        {
            try
            {
                var budget = await _context.Budgets.FindAsync(budgetId);
                if (budget != null)
                {
                    _context.Budgets.Remove(budget);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete budget", ex);
            }
        }
    }
}