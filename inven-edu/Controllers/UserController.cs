using inven_edu.Models.Entities;
using inven_edu.Models.ViewModels;
using inven_edu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace inven_edu.Controllers
{
    /// <summary>
    /// Controller for regular user functionality
    /// </summary>
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IIssuanceService _issuanceService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IInventoryService inventoryService,
            IIssuanceService issuanceService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger)
        {
            _inventoryService = inventoryService;
            _issuanceService = issuanceService;
            _categoryService = categoryService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Displays the user dashboard
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var viewModel = new UserDashboardViewModel
                {
                    UserName = currentUser.FullName
                };

                // Get user's issuances
                var userIssuances = await _issuanceService.GetUserIssuancesAsync(currentUser.Id);
                
                viewModel.TotalItemsIssued = userIssuances.Count();
                viewModel.CurrentlyHolding = userIssuances.Count(i => i.Status == IssuanceStatus.Issued);
                viewModel.OverdueItems = userIssuances.Count(i => i.IsOverdue);

                // Get active issuances
                viewModel.ActiveIssuances = userIssuances
                    .Where(i => i.Status == IssuanceStatus.Issued)
                    .OrderByDescending(i => i.IssuedDate)
                    .Select(MapToIssuanceViewModel)
                    .ToList();

                // Get recent issuances
                viewModel.RecentIssuances = userIssuances
                    .OrderByDescending(i => i.IssuedDate)
                    .Take(10)
                    .Select(MapToIssuanceViewModel)
                    .ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View(new UserDashboardViewModel());
            }
        }

        /// <summary>
        /// Displays the user's issuance history
        /// </summary>
        public async Task<IActionResult> MyIssuances(string? status, int pageNumber = 1)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var userIssuances = await _issuanceService.GetUserIssuancesAsync(currentUser.Id);

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status))
                {
                    userIssuances = userIssuances.Where(i => i.Status == status);
                }

                var pageSize = 10;
                var totalCount = userIssuances.Count();

                var viewModel = new IssuanceSearchViewModel
                {
                    Status = status,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    Issuances = userIssuances
                        .OrderByDescending(i => i.IssuedDate)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(MapToIssuanceViewModel)
                        .ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user issuances");
                TempData["Error"] = "An error occurred while loading your issuances.";
                return View(new IssuanceSearchViewModel());
            }
        }

        /// <summary>
        /// Displays the inventory browse page
        /// </summary>
        public async Task<IActionResult> BrowseInventory(string? searchTerm, int? categoryId, int pageNumber = 1)
        {
            try
            {
                var viewModel = new InventorySearchViewModel
                {
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    PageNumber = pageNumber,
                    PageSize = 10
                };

                var (items, totalCount) = await _inventoryService.SearchItemsAsync(
                    searchTerm, categoryId, false, false, pageNumber, viewModel.PageSize);

                viewModel.Items = items.Select(item => new InventoryItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    CategoryId = item.CategoryId,
                    CategoryName = item.Category?.Name,
                    MinimumStock = item.MinimumStock,
                    CreatedDate = item.CreatedDate,
                    UpdatedDate = item.UpdatedDate,
                    IsLowStock = item.IsLowStock,
                    IsOutOfStock = item.IsOutOfStock
                }).ToList();

                viewModel.TotalItems = totalCount;

                // Get categories for filter dropdown
                var categories = await _categoryService.GetAllCategoriesAsync();
                viewModel.Categories = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error browsing inventory");
                TempData["Error"] = "An error occurred while browsing inventory.";
                return View(new InventorySearchViewModel());
            }
        }

        /// <summary>
        /// Displays the item details
        /// </summary>
        public async Task<IActionResult> ItemDetails(int id)
        {
            try
            {
                var item = await _inventoryService.GetItemByIdAsync(id);
                if (item == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(BrowseInventory));
                }

                var viewModel = new InventoryItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    CategoryId = item.CategoryId,
                    CategoryName = item.Category?.Name,
                    MinimumStock = item.MinimumStock,
                    CreatedDate = item.CreatedDate,
                    UpdatedDate = item.UpdatedDate,
                    IsLowStock = item.IsLowStock,
                    IsOutOfStock = item.IsOutOfStock
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading item details");
                TempData["Error"] = "An error occurred while loading item details.";
                return RedirectToAction(nameof(BrowseInventory));
            }
        }

        private static IssuanceViewModel MapToIssuanceViewModel(IssuanceRecord issuance)
        {
            return new IssuanceViewModel
            {
                Id = issuance.Id,
                ItemName = issuance.Item?.Name ?? "N/A",
                UserName = issuance.User?.FullName ?? "N/A",
                UserEmail = issuance.User?.Email ?? "N/A",
                QuantityIssued = issuance.QuantityIssued,
                IssuedDate = issuance.IssuedDate,
                ReturnDate = issuance.ReturnDate,
                Status = issuance.Status,
                Notes = issuance.Notes,
                IsOverdue = issuance.IsOverdue
            };
        }
    }
}
