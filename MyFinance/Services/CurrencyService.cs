using Microsoft.EntityFrameworkCore;
using MyFinance.Models;

namespace MyFinance.Services
{
    public class CurrencyService
    {
        private readonly AppDbContext _context;

        public CurrencyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Currency>> GetAllCurrenciesAsync()
        {
            try
            {
                return await _context.Currencies.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get currencies", ex);
            }
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                var fromRate = (await _context.Currencies.FirstOrDefaultAsync(c => c.Code == fromCurrency))?.Rate ?? 1;
                var toRate = (await _context.Currencies.FirstOrDefaultAsync(c => c.Code == toCurrency))?.Rate ?? 1;
                return fromRate / toRate;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get exchange rate", ex);
            }
        }
    }
}