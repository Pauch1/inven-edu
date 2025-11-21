using inven_edu.Models.Entities;

namespace inven_edu.Services
{
    /// <summary>
    /// Interface for issuance management services
    /// </summary>
    public interface IIssuanceService
    {
        /// <summary>
        /// Gets all issuance records
        /// </summary>
        Task<IEnumerable<IssuanceRecord>> GetAllIssuancesAsync();

        /// <summary>
        /// Gets an issuance record by ID
        /// </summary>
        Task<IssuanceRecord?> GetIssuanceByIdAsync(int id);

        /// <summary>
        /// Creates a new issuance record
        /// </summary>
        Task<bool> CreateIssuanceAsync(IssuanceRecord issuance);

        /// <summary>
        /// Updates an existing issuance record
        /// </summary>
        Task<bool> UpdateIssuanceAsync(IssuanceRecord issuance);

        /// <summary>
        /// Marks an issuance as returned
        /// </summary>
        Task<bool> MarkAsReturnedAsync(int id);

        /// <summary>
        /// Gets issuances for a specific user
        /// </summary>
        Task<IEnumerable<IssuanceRecord>> GetUserIssuancesAsync(string userId);

        /// <summary>
        /// Gets active issuances
        /// </summary>
        Task<IEnumerable<IssuanceRecord>> GetActiveIssuancesAsync();

        /// <summary>
        /// Gets overdue issuances
        /// </summary>
        Task<IEnumerable<IssuanceRecord>> GetOverdueIssuancesAsync();

        /// <summary>
        /// Gets recent issuances
        /// </summary>
        Task<IEnumerable<IssuanceRecord>> GetRecentIssuancesAsync(int count);

        /// <summary>
        /// Searches issuances with filters
        /// </summary>
        Task<(IEnumerable<IssuanceRecord> issuances, int totalCount)> SearchIssuancesAsync(
            string? searchTerm,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Gets issuance statistics
        /// </summary>
        Task<(int total, int active, int overdue)> GetIssuanceStatisticsAsync();
    }
}
