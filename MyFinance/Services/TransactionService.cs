using Microsoft.EntityFrameworkCore;
using MyFinance.Models;

namespace MyFinance.Services
{
    public class TransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetTransactionsByBudgetIdAsync(int budgetId)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.BudgetId == budgetId)
                    .Include(t => t.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get transactions", ex);
            }
        }

        public async Task<List<Transaction>> GetTransactionsByDateRangeAsync(int budgetId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Transactions
                    .Where(t => t.BudgetId == budgetId && t.Date >= startDate && t.Date <= endDate)
                    .Include(t => t.Category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get transactions by date range", ex);
            }
        }

        public async Task<Transaction> CreateTransactionAsync(int budgetId, int categoryId, string description, decimal amount, TransactionType type, DateTime date)
        {
            try
            {
                var transaction = new Transaction
                {
                    BudgetId = budgetId,
                    CategoryId = categoryId,
                    Description = description,
                    Amount = amount,
                    Type = type,
                    Date = date
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                var budget = await _context.Budgets.FindAsync(budgetId);
                if (type == TransactionType.Income)
                {
                    budget.TotalIncome += amount;
                    budget.Balance += amount; // Збільшуємо баланс для доходу
                }
                else
                {
                    budget.TotalExpense += amount;
                    budget.Balance -= amount; // Зменшуємо баланс для витрати
                }
                await _context.SaveChangesAsync();

                return transaction;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create transaction", ex);
            }
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            try
            {
                var oldTransaction = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == transaction.Id);
                _context.Transactions.Update(transaction);
                await _context.SaveChangesAsync();

                var budget = await _context.Budgets.FindAsync(transaction.BudgetId);
                // Скасування ефекту старої транзакції
                if (oldTransaction.Type == TransactionType.Income)
                {
                    budget.TotalIncome -= oldTransaction.Amount;
                    budget.Balance -= oldTransaction.Amount; // Скасування доходу
                }
                else
                {
                    budget.TotalExpense -= oldTransaction.Amount;
                    budget.Balance += oldTransaction.Amount; // Скасування витрати
                }
                // Застосування ефекту нової транзакції
                if (transaction.Type == TransactionType.Income)
                {
                    budget.TotalIncome += transaction.Amount;
                    budget.Balance += transaction.Amount; // Збільшення балансу для доходу
                }
                else
                {
                    budget.TotalExpense += transaction.Amount;
                    budget.Balance -= transaction.Amount; // Зменшення балансу для витрати
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update transaction", ex);
            }
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(transactionId);
                if (transaction != null)
                {
                    _context.Transactions.Remove(transaction);
                    await _context.SaveChangesAsync();

                    var budget = await _context.Budgets.FindAsync(transaction.BudgetId);
                    if (transaction.Type == TransactionType.Income)
                    {
                        budget.TotalIncome -= transaction.Amount;
                        budget.Balance -= transaction.Amount; // Зменшення балансу при видаленні доходу
                    }
                    else
                    {
                        budget.TotalExpense -= transaction.Amount;
                        budget.Balance += transaction.Amount; // Збільшення балансу при видаленні витрати
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete transaction", ex);
            }
        }
    }
}