using inven_edu.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace inven_edu.Data
{
    /// <summary>
    /// Seeds initial data into the database
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Seeds the database with initial data including admin user and sample inventory
        /// </summary>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync(roleManager);

            // Seed admin user
            await SeedAdminUserAsync(userManager);

            // Seed sample users
            await SeedSampleUsersAsync(userManager);

            // Seed categories
            await SeedCategoriesAsync(context);

            // Seed inventory items
            await SeedInventoryItemsAsync(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@invenedu.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedSampleUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var sampleUsers = new[]
            {
                new { Email = "john.doe@university.edu", FirstName = "John", LastName = "Doe" },
                new { Email = "jane.smith@university.edu", FirstName = "Jane", LastName = "Smith" },
                new { Email = "mike.wilson@university.edu", FirstName = "Mike", LastName = "Wilson" },
                new { Email = "sarah.johnson@university.edu", FirstName = "Sarah", LastName = "Johnson" }
            };

            foreach (var userInfo in sampleUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(userInfo.Email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(user, "User123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category { Name = "Office Supplies", Description = "General office supplies and stationery", CreatedDate = DateTime.UtcNow },
                    new Category { Name = "Electronics", Description = "Electronic devices and equipment", CreatedDate = DateTime.UtcNow },
                    new Category { Name = "Furniture", Description = "Office and classroom furniture", CreatedDate = DateTime.UtcNow },
                    new Category { Name = "Laboratory Equipment", Description = "Scientific and laboratory equipment", CreatedDate = DateTime.UtcNow },
                    new Category { Name = "Books & Publications", Description = "Books, journals, and publications", CreatedDate = DateTime.UtcNow },
                    new Category { Name = "Sports Equipment", Description = "Sports and physical education equipment", CreatedDate = DateTime.UtcNow },
                    new Category { Name = "IT Equipment", Description = "Computers, peripherals, and IT hardware", CreatedDate = DateTime.UtcNow }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedInventoryItemsAsync(ApplicationDbContext context)
        {
            if (!await context.InventoryItems.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var random = new Random();

                var items = new List<InventoryItem>();

                // Office Supplies
                var officeSupplies = categories.FirstOrDefault(c => c.Name == "Office Supplies");
                if (officeSupplies != null)
                {
                    items.AddRange(new[]
                    {
                        new InventoryItem { Name = "A4 Paper Ream", Description = "500 sheets white A4 paper", Quantity = 150, CategoryId = officeSupplies.Id, MinimumStock = 20, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Blue Pens", Description = "Ballpoint pens, blue ink", Quantity = 300, CategoryId = officeSupplies.Id, MinimumStock = 50, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Black Markers", Description = "Permanent markers, black", Quantity = 75, CategoryId = officeSupplies.Id, MinimumStock = 15, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Staplers", Description = "Standard office staplers", Quantity = 25, CategoryId = officeSupplies.Id, MinimumStock = 5, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "File Folders", Description = "Manila file folders", Quantity = 200, CategoryId = officeSupplies.Id, MinimumStock = 30, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Notebooks", Description = "A5 spiral notebooks, 100 pages", Quantity = 120, CategoryId = officeSupplies.Id, MinimumStock = 25, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
                    });
                }

                // Electronics
                var electronics = categories.FirstOrDefault(c => c.Name == "Electronics");
                if (electronics != null)
                {
                    items.AddRange(new[]
                    {
                        new InventoryItem { Name = "Projectors", Description = "LCD projectors for classrooms", Quantity = 15, CategoryId = electronics.Id, MinimumStock = 3, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Calculators", Description = "Scientific calculators", Quantity = 45, CategoryId = electronics.Id, MinimumStock = 10, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "USB Flash Drives 32GB", Description = "32GB USB 3.0 flash drives", Quantity = 60, CategoryId = electronics.Id, MinimumStock = 15, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Extension Cords", Description = "6-outlet extension cords", Quantity = 30, CategoryId = electronics.Id, MinimumStock = 8, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
                    });
                }

                // IT Equipment
                var itEquipment = categories.FirstOrDefault(c => c.Name == "IT Equipment");
                if (itEquipment != null)
                {
                    items.AddRange(new[]
                    {
                        new InventoryItem { Name = "Laptop Computers", Description = "Dell Latitude laptops", Quantity = 8, CategoryId = itEquipment.Id, MinimumStock = 5, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Wireless Mice", Description = "Wireless optical mice", Quantity = 35, CategoryId = itEquipment.Id, MinimumStock = 10, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Keyboards", Description = "USB wired keyboards", Quantity = 40, CategoryId = itEquipment.Id, MinimumStock = 10, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "HDMI Cables", Description = "6ft HDMI cables", Quantity = 25, CategoryId = itEquipment.Id, MinimumStock = 8, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Webcams", Description = "HD USB webcams", Quantity = 5, CategoryId = itEquipment.Id, MinimumStock = 3, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
                    });
                }

                // Laboratory Equipment
                var labEquipment = categories.FirstOrDefault(c => c.Name == "Laboratory Equipment");
                if (labEquipment != null)
                {
                    items.AddRange(new[]
                    {
                        new InventoryItem { Name = "Microscopes", Description = "Compound microscopes", Quantity = 12, CategoryId = labEquipment.Id, MinimumStock = 3, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Safety Goggles", Description = "Lab safety goggles", Quantity = 50, CategoryId = labEquipment.Id, MinimumStock = 15, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Test Tubes", Description = "Glass test tubes, 16mm", Quantity = 200, CategoryId = labEquipment.Id, MinimumStock = 40, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Lab Coats", Description = "White laboratory coats", Quantity = 18, CategoryId = labEquipment.Id, MinimumStock = 5, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
                    });
                }

                // Sports Equipment
                var sportsEquipment = categories.FirstOrDefault(c => c.Name == "Sports Equipment");
                if (sportsEquipment != null)
                {
                    items.AddRange(new[]
                    {
                        new InventoryItem { Name = "Basketballs", Description = "Official size basketballs", Quantity = 20, CategoryId = sportsEquipment.Id, MinimumStock = 5, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Soccer Balls", Description = "Size 5 soccer balls", Quantity = 15, CategoryId = sportsEquipment.Id, MinimumStock = 5, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Yoga Mats", Description = "Non-slip yoga mats", Quantity = 30, CategoryId = sportsEquipment.Id, MinimumStock = 10, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
                    });
                }

                // Furniture
                var furniture = categories.FirstOrDefault(c => c.Name == "Furniture");
                if (furniture != null)
                {
                    items.AddRange(new[]
                    {
                        new InventoryItem { Name = "Student Desks", Description = "Single student desks", Quantity = 50, CategoryId = furniture.Id, MinimumStock = 10, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Office Chairs", Description = "Ergonomic office chairs", Quantity = 25, CategoryId = furniture.Id, MinimumStock = 5, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow },
                        new InventoryItem { Name = "Whiteboards", Description = "Wall-mounted whiteboards", Quantity = 10, CategoryId = furniture.Id, MinimumStock = 2, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow }
                    });
                }

                await context.InventoryItems.AddRangeAsync(items);
                await context.SaveChangesAsync();
            }
        }
    }
}
