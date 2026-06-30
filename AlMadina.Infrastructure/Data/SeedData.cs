using AlMadina.Domain.Entities;
using AlMadina.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlMadina.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            // =====================
            // ROLES
            // =====================
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // =====================
            // ADMIN
            // =====================
            var adminEmail = "Mohamedelhnawey676@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "7nawey",
                    Email = adminEmail,
                    FullName = "Mohamed Elhnawey",
                    PhoneNumber = "01200289439",
                    Address = "Al Madinah",
                    IsActive = true,
                    Role = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin123@M");

                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                adminUser = await userManager.FindByEmailAsync(adminEmail);
                await userManager.AddToRoleAsync(adminUser!, "Admin");
            }

            // =====================
            // USER 1
            // =====================
            var user1Email = "user1@almadina.com";
            var user1 = await userManager.FindByEmailAsync(user1Email);

            if (user1 == null)
            {
                var newUser1 = new ApplicationUser
                {
                    UserName = "user1",
                    Email = user1Email,
                    FullName = "User One",
                    PhoneNumber = "01000000001",
                    Address = "Cairo",
                    IsActive = true,
                    Role = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newUser1, "User123@M");

                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                user1 = await userManager.FindByEmailAsync(user1Email);
                await userManager.AddToRoleAsync(user1!, "User");
            }

            // =====================
            // USER 2
            // =====================
            var user2Email = "user2@almadina.com";
            var user2 = await userManager.FindByEmailAsync(user2Email);

            if (user2 == null)
            {
                var newUser2 = new ApplicationUser
                {
                    UserName = "user2",
                    Email = user2Email,
                    FullName = "User Two",
                    PhoneNumber = "01000000002",
                    Address = "Alexandria",
                    IsActive = true,
                    Role = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newUser2, "User123@M");

                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                user2 = await userManager.FindByEmailAsync(user2Email);
                await userManager.AddToRoleAsync(user2!, "User");
            }

            // =====================
            // CATEGORIES
            // =====================
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Id = Guid.NewGuid(), NameAr = "مشروبات", NameEn = "Beverages", Description = "مشروبات غازية وعصائر و مياه", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "مخبوزات", NameEn = "Bakery", Description = "خبز وكعك ومعجنات", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "حلويات", NameEn = "Sweets", Description = "حلويات وشيكولاتة وبسكويت", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "ألبان وأجبان", NameEn = "Dairy", Description = "حليب و جبن و زبادي", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "لحوم ودواجن", NameEn = "Meat", Description = "لحوم طازجة ودواجن", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "منظفات", NameEn = "Cleaning", Description = "منظفات المنزل والمطبخ", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "معلبات", NameEn = "Canned", Description = "معلبات ومواد غذائية جافة", IsActive = true },
                    new() { Id = Guid.NewGuid(), NameAr = "خضروات وفاكهة", NameEn = "Vegetables", Description = "خضروات وفاكهة طازجة", IsActive = true }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // =====================
            // PRODUCTS
            // =====================
            if (!await context.Products.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();

                var beverages = categories.First(c => c.NameEn == "Beverages");
                var bakery = categories.First(c => c.NameEn == "Bakery");
                var sweets = categories.First(c => c.NameEn == "Sweets");
                var dairy = categories.First(c => c.NameEn == "Dairy");
                var meat = categories.First(c => c.NameEn == "Meat");
                var cleaning = categories.First(c => c.NameEn == "Cleaning");
                var canned = categories.First(c => c.NameEn == "Canned");
                var vegetables = categories.First(c => c.NameEn == "Vegetables");

                var products = new List<Product>
                {
                    new() { Id = Guid.NewGuid(), NameAr="بيبسي 1 لتر", NameEn="Pepsi 1L", Barcode="123456789001", Price=15, CostPrice=10, StockQuantity=50, CategoryId=beverages.Id, IsFeatured=true, DiscountPercentage=10, IsActive=true },
                    new() { Id = Guid.NewGuid(), NameAr="سفن أب 1 لتر", NameEn="7UP 1L", Barcode="123456789002", Price=14, CostPrice=9, StockQuantity=45, CategoryId=beverages.Id, IsFeatured=false, DiscountPercentage=0, IsActive=true },
                    new() { Id = Guid.NewGuid(), NameAr="خبز بلدي", NameEn="Baladi Bread", Barcode="123456789003", Price=5, CostPrice=2, StockQuantity=200, CategoryId=bakery.Id, IsFeatured=true, DiscountPercentage=0, IsActive=true },
                    new() { Id = Guid.NewGuid(), NameAr="كيك شوكولاتة", NameEn="Chocolate Cake", Barcode="123456789004", Price=35, CostPrice=20, StockQuantity=15, CategoryId=sweets.Id, IsFeatured=true, DiscountPercentage=20, IsActive=true }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}