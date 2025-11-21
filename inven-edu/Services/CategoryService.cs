using inven_edu.Data;
using inven_edu.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace inven_edu.Services
{
    /// <summary>
    /// Implementation of category management services
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.InventoryItems)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories");
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.InventoryItems)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                // Check if category name already exists
                if (await CategoryNameExistsAsync(category.Name))
                {
                    _logger.LogWarning("Attempted to create category with duplicate name: {CategoryName}", category.Name);
                    return false;
                }

                category.CreatedDate = DateTime.UtcNow;
                await _context.Categories.AddAsync(category);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Created new category: {CategoryName}", category.Name);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {CategoryName}", category.Name);
                throw;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(category.Id);
                if (existingCategory == null)
                {
                    _logger.LogWarning("Attempted to update non-existent category with ID {CategoryId}", category.Id);
                    return false;
                }

                // Check if new name conflicts with another category
                if (await CategoryNameExistsAsync(category.Name, category.Id))
                {
                    _logger.LogWarning("Attempted to update category to duplicate name: {CategoryName}", category.Name);
                    return false;
                }

                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;

                _context.Categories.Update(existingCategory);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Updated category: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {CategoryId}", category.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.InventoryItems)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent category with ID {CategoryId}", id);
                    return false;
                }

                // Check if category has any inventory items
                if (category.InventoryItems.Any())
                {
                    _logger.LogWarning("Cannot delete category {CategoryId} because it has {ItemCount} inventory items",
                        id, category.InventoryItems.Count);
                    return false;
                }

                _context.Categories.Remove(category);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted category: {CategoryName} (ID: {CategoryId})", category.Name, id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null)
        {
            try
            {
                var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
                
                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if category name exists: {CategoryName}", name);
                throw;
            }
        }
    }
}
