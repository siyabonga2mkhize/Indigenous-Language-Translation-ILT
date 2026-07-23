using System.Collections.Generic;
using System.Threading.Tasks;
using InnoDevsITL.Data.Repositories.Interfaces;
using InnoDevsITL.Models;
using InnoDevsITL.Services.Interfaces;

namespace InnoDevsITL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<Category> CreateCategoryAsync(Category category)
        {
            return _categoryRepository.AddAsync(category);
        }

        public Task<bool> DeleteCategoryAsync(int id)
        {
            return _categoryRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return _categoryRepository.GetAllAsync();
        }

        public Task<Category> GetByIdAsync(int id)
        {
            return _categoryRepository.GetByIdAsync(id);
        }

        public Task<Category> UpdateCategoryAsync(Category category)
        {
            return _categoryRepository.UpdateAsync(category);
        }
    }
}
