using InnoDevsITL.Data;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InnoDevsITL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly InnoDbContext _context;

        public CategoryService(InnoDbContext context)
        {
            _context = context;
        }

        // Match interface exactly: GetAllCategoriesAsync
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Match interface exactly: GetCategoryByIdAsync
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithPhrasesAsync()
        {
            return await _context.Categories
                .Include(c => c.Phrases)
                .ToListAsync();
        }
    }
}