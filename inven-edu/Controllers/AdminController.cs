using inven_edu.Models.Entities;
using inven_edu.Models.ViewModels;
using inven_edu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace inven_edu.Controllers
{
    /// <summary>
    /// Controller for admin-specific functionality
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IIssuanceService _issuanceService;
        private readonly ICategoryService _categoryService;
        private readonly IPdfService _pdfService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IInventoryService inventoryService,
            IIssuanceService issuanceService,
            ICategoryService categoryService,
            IPdfService pdfService,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminController> logger)
        {
            _inventoryService = inventoryService;
            _issuanceService = issuanceService;
            _categoryService = categoryService;
            _pdfService = pdfService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Displays the admin dashboard
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var viewModel = new AdminDashboardViewModel();

                // Get inventory statistics
                var allItems = await _inventoryService.GetAllItemsAsync();
                viewModel.TotalItems = allItems.Count();
                viewModel.LowStockItems = allItems.Count(i => i.IsLowStock && !i.IsOutOfStock);
                viewModel.OutOfStockItems = allItems.Count(i => i.IsOutOfStock);

                // Get issuance statistics
                var stats = await _issuanceService.GetIssuanceStatisticsAsync();
                viewModel.ActiveIssuances = stats.active;
                viewModel.OverdueIssuances = stats.overdue;

                // Get user count
                viewModel.TotalUsers = _userManager.Users.Count();

                // Get recent issuances
                var recentIssuances = await _issuanceService.GetRecentIssuancesAsync(5);
                viewModel.RecentIssuances = recentIssuances.Select(MapToIssuanceViewModel).ToList();

                // Get low stock items
                var lowStockItems = await _inventoryService.GetLowStockItemsAsync();
                viewModel.LowStockItemsList = lowStockItems.Select(MapToInventoryViewModel).Take(5).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View(new AdminDashboardViewModel());
            }
        }

        #region Inventory Management

        /// <summary>
        /// Displays the inventory list with search and filters
        /// </summary>
        public async Task<IActionResult> Inventory(string? searchTerm, int? categoryId, bool? lowStockOnly, bool? outOfStockOnly, int pageNumber = 1)
        {
            try
            {
                var viewModel = new InventorySearchViewModel
                {
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    LowStockOnly = lowStockOnly,
                    OutOfStockOnly = outOfStockOnly,
                    PageNumber = pageNumber,
                    PageSize = 10
                };

                var (items, totalCount) = await _inventoryService.SearchItemsAsync(
                    searchTerm, categoryId, lowStockOnly, outOfStockOnly, pageNumber, viewModel.PageSize);

                viewModel.Items = items.Select(MapToInventoryViewModel).ToList();
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
                _logger.LogError(ex, "Error loading inventory");
                TempData["Error"] = "An error occurred while loading inventory.";
                return View(new InventorySearchViewModel());
            }
        }

        /// <summary>
        /// Displays the form to create a new inventory item
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateItem()
        {
            await PopulateCategoriesDropdown();
            return View(new InventoryItemViewModel());
        }

        /// <summary>
        /// Processes the creation of a new inventory item
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(InventoryItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesDropdown();
                return View(model);
            }

            try
            {
                var item = new InventoryItem
                {
                    Name = model.Name,
                    Description = model.Description,
                    Quantity = model.Quantity,
                    CategoryId = model.CategoryId,
                    MinimumStock = model.MinimumStock
                };

                var result = await _inventoryService.CreateItemAsync(item);

                if (result)
                {
                    TempData["Success"] = "Inventory item created successfully.";
                    return RedirectToAction(nameof(Inventory));
                }

                ModelState.AddModelError("", "Failed to create inventory item.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item");
                ModelState.AddModelError("", "An error occurred while creating the item.");
            }

            await PopulateCategoriesDropdown();
            return View(model);
        }

        /// <summary>
        /// Displays the form to edit an inventory item
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            try
            {
                var item = await _inventoryService.GetItemByIdAsync(id);
                if (item == null)
                {
                    TempData["Error"] = "Inventory item not found.";
                    return RedirectToAction(nameof(Inventory));
                }

                var viewModel = MapToInventoryViewModel(item);
                await PopulateCategoriesDropdown();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading item for editing");
                TempData["Error"] = "An error occurred while loading the item.";
                return RedirectToAction(nameof(Inventory));
            }
        }

        /// <summary>
        /// Processes the update of an inventory item
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(InventoryItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesDropdown();
                return View(model);
            }

            try
            {
                var item = new InventoryItem
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Quantity = model.Quantity,
                    CategoryId = model.CategoryId,
                    MinimumStock = model.MinimumStock
                };

                var result = await _inventoryService.UpdateItemAsync(item);

                if (result)
                {
                    TempData["Success"] = "Inventory item updated successfully.";
                    return RedirectToAction(nameof(Inventory));
                }

                ModelState.AddModelError("", "Failed to update inventory item.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item");
                ModelState.AddModelError("", "An error occurred while updating the item.");
            }

            await PopulateCategoriesDropdown();
            return View(model);
        }

        /// <summary>
        /// Deletes an inventory item
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var result = await _inventoryService.DeleteItemAsync(id);

                if (result)
                {
                    TempData["Success"] = "Inventory item deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete item. It may have issuance records.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory item");
                TempData["Error"] = "An error occurred while deleting the item.";
            }

            return RedirectToAction(nameof(Inventory));
        }

        /// <summary>
        /// Generates and downloads inventory PDF report
        /// </summary>
        public async Task<IActionResult> DownloadInventoryReport()
        {
            try
            {
                var items = await _inventoryService.GetAllItemsAsync();
                var pdfBytes = _pdfService.GenerateInventoryReport(items);

                return File(pdfBytes, "application/pdf", $"Inventory_Report_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating inventory report");
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction(nameof(Inventory));
            }
        }

        #endregion

        #region Issuance Management

        /// <summary>
        /// Displays the issuance list with search and filters
        /// </summary>
        public async Task<IActionResult> Issuances(string? searchTerm, string? status, DateTime? fromDate, DateTime? toDate, int pageNumber = 1)
        {
            try
            {
                var viewModel = new IssuanceSearchViewModel
                {
                    SearchTerm = searchTerm,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageNumber = pageNumber,
                    PageSize = 10
                };

                var (issuances, totalCount) = await _issuanceService.SearchIssuancesAsync(
                    searchTerm, status, fromDate, toDate, pageNumber, viewModel.PageSize);

                viewModel.Issuances = issuances.Select(MapToIssuanceViewModel).ToList();
                viewModel.TotalItems = totalCount;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading issuances");
                TempData["Error"] = "An error occurred while loading issuances.";
                return View(new IssuanceSearchViewModel());
            }
        }

        /// <summary>
        /// Displays the form to create a new issuance
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateIssuance()
        {
            var viewModel = new CreateIssuanceViewModel();
            await PopulateIssuanceDropdowns(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Processes the creation of a new issuance
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIssuance(CreateIssuanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateIssuanceDropdowns(model);
                return View(model);
            }

            try
            {
                // Check if sufficient quantity is available
                var hasSufficientQuantity = await _inventoryService.HasSufficientQuantityAsync(
                    model.ItemId, model.QuantityIssued);

                if (!hasSufficientQuantity)
                {
                    ModelState.AddModelError("", "Insufficient quantity available for this item.");
                    await PopulateIssuanceDropdowns(model);
                    return View(model);
                }

                var issuance = new IssuanceRecord
                {
                    ItemId = model.ItemId,
                    UserId = model.UserId,
                    QuantityIssued = model.QuantityIssued,
                    ReturnDate = model.ReturnDate,
                    Notes = model.Notes
                };

                var result = await _issuanceService.CreateIssuanceAsync(issuance);

                if (result)
                {
                    TempData["Success"] = "Item issued successfully.";
                    return RedirectToAction(nameof(Issuances));
                }

                ModelState.AddModelError("", "Failed to create issuance.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issuance");
                ModelState.AddModelError("", "An error occurred while creating the issuance.");
            }

            await PopulateIssuanceDropdowns(model);
            return View(model);
        }

        /// <summary>
        /// Marks an issuance as returned
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnIssuance(int id)
        {
            try
            {
                var result = await _issuanceService.MarkAsReturnedAsync(id);

                if (result)
                {
                    TempData["Success"] = "Item marked as returned successfully.";
                }
                else
                {
                    TempData["Error"] = "Unable to mark item as returned.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning issuance");
                TempData["Error"] = "An error occurred while processing the return.";
            }

            return RedirectToAction(nameof(Issuances));
        }

        /// <summary>
        /// Generates and downloads issuance PDF report
        /// </summary>
        public async Task<IActionResult> DownloadIssuanceReport()
        {
            try
            {
                var issuances = await _issuanceService.GetAllIssuancesAsync();
                var pdfBytes = _pdfService.GenerateIssuanceReport(issuances);

                return File(pdfBytes, "application/pdf", $"Issuance_Report_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating issuance report");
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction(nameof(Issuances));
            }
        }

        #endregion

        #region Category Management

        /// <summary>
        /// Displays the category list
        /// </summary>
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var viewModel = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedDate = c.CreatedDate,
                    ItemCount = c.InventoryItems.Count
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                TempData["Error"] = "An error occurred while loading categories.";
                return View(new List<CategoryViewModel>());
            }
        }

        /// <summary>
        /// Displays the form to create a new category
        /// </summary>
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View(new CategoryViewModel());
        }

        /// <summary>
        /// Processes the creation of a new category
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description
                };

                var result = await _categoryService.CreateCategoryAsync(category);

                if (result)
                {
                    TempData["Success"] = "Category created successfully.";
                    return RedirectToAction(nameof(Categories));
                }

                ModelState.AddModelError("", "Category name already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                ModelState.AddModelError("", "An error occurred while creating the category.");
            }

            return View(model);
        }

        /// <summary>
        /// Displays the form to edit a category
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Categories));
                }

                var viewModel = new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for editing");
                TempData["Error"] = "An error occurred while loading the category.";
                return RedirectToAction(nameof(Categories));
            }
        }

        /// <summary>
        /// Processes the update of a category
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var category = new Category
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description
                };

                var result = await _categoryService.UpdateCategoryAsync(category);

                if (result)
                {
                    TempData["Success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Categories));
                }

                ModelState.AddModelError("", "Category name already exists or update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                ModelState.AddModelError("", "An error occurred while updating the category.");
            }

            return View(model);
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (result)
                {
                    TempData["Success"] = "Category deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete category. It may have inventory items.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                TempData["Error"] = "An error occurred while deleting the category.";
            }

            return RedirectToAction(nameof(Categories));
        }

        #endregion

        #region User Management

        /// <summary>
        /// Displays the user management page
        /// </summary>
        public async Task<IActionResult> Users(string? searchTerm, bool? activeOnly)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(u => u.FirstName.Contains(searchTerm) ||
                                           u.LastName.Contains(searchTerm) ||
                                           u.Email.Contains(searchTerm));
                }

                if (activeOnly == true)
                {
                    query = query.Where(u => u.IsActive);
                }

                var users = query.ToList();
                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var issuanceCount = user.IssuanceRecords.Count;

                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email ?? "",
                        Role = roles.FirstOrDefault() ?? "User",
                        CreatedDate = user.CreatedDate,
                        IsActive = user.IsActive,
                        TotalIssuances = issuanceCount
                    });
                }

                var viewModel = new ManageUsersViewModel
                {
                    Users = userViewModels,
                    SearchTerm = searchTerm,
                    ActiveOnly = activeOnly
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                TempData["Error"] = "An error occurred while loading users.";
                return View(new ManageUsersViewModel());
            }
        }

        #endregion

        #region Helper Methods

        private async Task PopulateCategoriesDropdown()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        private async Task PopulateIssuanceDropdowns(CreateIssuanceViewModel model)
        {
            var items = await _inventoryService.GetAllItemsAsync();
            model.AvailableItems = items.Where(i => i.Quantity > 0)
                .Select(MapToInventoryViewModel).ToList();

            var users = _userManager.Users.Where(u => u.IsActive).ToList();
            model.AvailableUsers = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email ?? ""
            }).ToList();

            if (model.ItemId > 0)
            {
                var item = await _inventoryService.GetItemByIdAsync(model.ItemId);
                model.AvailableQuantity = item?.Quantity;
            }
        }

        private static InventoryItemViewModel MapToInventoryViewModel(InventoryItem item)
        {
            return new InventoryItemViewModel
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

        #endregion
    }
}
