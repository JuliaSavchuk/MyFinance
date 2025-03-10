using Microsoft.EntityFrameworkCore;
using MyFinance.Models;

namespace MyFinance.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.Categories.Include(c => c.Transactions).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get categories", ex);
            }
        }

        public void AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add category", ex);
            }
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update category", ex);
            }
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete category", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save changes", ex);
            }
        }
    }
}
