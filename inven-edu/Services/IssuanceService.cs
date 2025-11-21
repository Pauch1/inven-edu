using inven_edu.Data;
using inven_edu.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace inven_edu.Services
{
    /// <summary>
    /// Implementation of issuance management services
    /// </summary>
    public class IssuanceService : IIssuanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IssuanceService> _logger;
        private readonly IInventoryService _inventoryService;

        public IssuanceService(
            ApplicationDbContext context, 
            ILogger<IssuanceService> logger,
            IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _inventoryService = inventoryService;
        }

        public async Task<IEnumerable<IssuanceRecord>> GetAllIssuancesAsync()
        {
            try
            {
                return await _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .OrderByDescending(ir => ir.IssuedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all issuances");
                throw;
            }
        }

        public async Task<IssuanceRecord?> GetIssuanceByIdAsync(int id)
        {
            try
            {
                return await _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .FirstOrDefaultAsync(ir => ir.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issuance with ID {IssuanceId}", id);
                throw;
            }
        }

        public async Task<bool> CreateIssuanceAsync(IssuanceRecord issuance)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if sufficient quantity is available
                var hasSufficientQuantity = await _inventoryService.HasSufficientQuantityAsync(
                    issuance.ItemId, 
                    issuance.QuantityIssued);

                if (!hasSufficientQuantity)
                {
                    _logger.LogWarning("Insufficient quantity for item {ItemId}. Requested: {Quantity}", 
                        issuance.ItemId, issuance.QuantityIssued);
                    return false;
                }

                // Create the issuance record
                issuance.IssuedDate = DateTime.UtcNow;
                issuance.Status = IssuanceStatus.Issued;

                await _context.IssuanceRecords.AddAsync(issuance);
                await _context.SaveChangesAsync();

                // Update inventory quantity
                var item = await _context.InventoryItems.FindAsync(issuance.ItemId);
                if (item != null)
                {
                    item.Quantity -= issuance.QuantityIssued;
                    item.UpdatedDate = DateTime.UtcNow;
                    _context.InventoryItems.Update(item);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation("Created issuance for item {ItemId} to user {UserId}. Quantity: {Quantity}",
                    issuance.ItemId, issuance.UserId, issuance.QuantityIssued);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating issuance");
                throw;
            }
        }

        public async Task<bool> UpdateIssuanceAsync(IssuanceRecord issuance)
        {
            try
            {
                var existingIssuance = await _context.IssuanceRecords.FindAsync(issuance.Id);
                if (existingIssuance == null)
                {
                    _logger.LogWarning("Attempted to update non-existent issuance with ID {IssuanceId}", issuance.Id);
                    return false;
                }

                existingIssuance.Status = issuance.Status;
                existingIssuance.ReturnDate = issuance.ReturnDate;
                existingIssuance.Notes = issuance.Notes;

                _context.IssuanceRecords.Update(existingIssuance);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Updated issuance with ID {IssuanceId}", issuance.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issuance with ID {IssuanceId}", issuance.Id);
                throw;
            }
        }

        public async Task<bool> MarkAsReturnedAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var issuance = await _context.IssuanceRecords.FindAsync(id);
                if (issuance == null)
                {
                    _logger.LogWarning("Attempted to mark non-existent issuance as returned. ID: {IssuanceId}", id);
                    return false;
                }

                if (issuance.Status == IssuanceStatus.Returned)
                {
                    _logger.LogWarning("Issuance {IssuanceId} is already marked as returned", id);
                    return false;
                }

                // Update issuance status
                issuance.Status = IssuanceStatus.Returned;
                issuance.ReturnDate = DateTime.UtcNow;
                _context.IssuanceRecords.Update(issuance);
                await _context.SaveChangesAsync();

                // Return items to inventory
                var item = await _context.InventoryItems.FindAsync(issuance.ItemId);
                if (item != null)
                {
                    item.Quantity += issuance.QuantityIssued;
                    item.UpdatedDate = DateTime.UtcNow;
                    _context.InventoryItems.Update(item);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation("Marked issuance {IssuanceId} as returned. Returned {Quantity} items to inventory",
                    id, issuance.QuantityIssued);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error marking issuance as returned. ID: {IssuanceId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<IssuanceRecord>> GetUserIssuancesAsync(string userId)
        {
            try
            {
                return await _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .Where(ir => ir.UserId == userId)
                    .OrderByDescending(ir => ir.IssuedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issuances for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<IssuanceRecord>> GetActiveIssuancesAsync()
        {
            try
            {
                return await _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .Where(ir => ir.Status == IssuanceStatus.Issued)
                    .OrderByDescending(ir => ir.IssuedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active issuances");
                throw;
            }
        }

        public async Task<IEnumerable<IssuanceRecord>> GetOverdueIssuancesAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .Where(ir => ir.Status == IssuanceStatus.Issued && 
                               ir.ReturnDate.HasValue && 
                               ir.ReturnDate.Value < now)
                    .OrderBy(ir => ir.ReturnDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue issuances");
                throw;
            }
        }

        public async Task<IEnumerable<IssuanceRecord>> GetRecentIssuancesAsync(int count)
        {
            try
            {
                return await _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .OrderByDescending(ir => ir.IssuedDate)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent issuances");
                throw;
            }
        }

        public async Task<(IEnumerable<IssuanceRecord> issuances, int totalCount)> SearchIssuancesAsync(
            string? searchTerm,
            string? status,
            DateTime? fromDate,
            DateTime? toDate,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var query = _context.IssuanceRecords
                    .Include(ir => ir.Item)
                    .Include(ir => ir.User)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(ir => 
                        ir.Item.Name.Contains(searchTerm) ||
                        ir.User.FirstName.Contains(searchTerm) ||
                        ir.User.LastName.Contains(searchTerm) ||
                        ir.User.Email.Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(ir => ir.Status == status);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(ir => ir.IssuedDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(ir => ir.IssuedDate <= toDate.Value);
                }

                var totalCount = await query.CountAsync();

                var issuances = await query
                    .OrderByDescending(ir => ir.IssuedDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (issuances, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching issuances");
                throw;
            }
        }

        public async Task<(int total, int active, int overdue)> GetIssuanceStatisticsAsync()
        {
            try
            {
                var total = await _context.IssuanceRecords.CountAsync();
                var active = await _context.IssuanceRecords.CountAsync(ir => ir.Status == IssuanceStatus.Issued);
                
                var now = DateTime.UtcNow;
                var overdue = await _context.IssuanceRecords.CountAsync(ir => 
                    ir.Status == IssuanceStatus.Issued && 
                    ir.ReturnDate.HasValue && 
                    ir.ReturnDate.Value < now);

                return (total, active, overdue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving issuance statistics");
                throw;
            }
        }
    }
}
