using inven_edu.Models.Entities;

namespace inven_edu.Services
{
    /// <summary>
    /// Interface for category management services
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Gets all categories
        /// </summary>
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        Task<Category?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Creates a new category
        /// </summary>
        Task<bool> CreateCategoryAsync(Category category);

        /// <summary>
        /// Updates an existing category
        /// </summary>
        Task<bool> UpdateCategoryAsync(Category category);

        /// <summary>
        /// Deletes a category
        /// </summary>
        Task<bool> DeleteCategoryAsync(int id);

        /// <summary>
        /// Checks if a category name already exists
        /// </summary>
        Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null);
    }
}
