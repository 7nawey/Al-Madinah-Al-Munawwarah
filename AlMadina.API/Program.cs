using AlMadina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using AlMadina.Application.Interfaces;
using AlMadina.Infrastructure.Repositories;

using AlMadina.Infrastructure.Services;

using AlMadina.Domain.Entities;
using AlMadina.Application.Interfaces.Services;


namespace AlMadina.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================
            // Database
            // =========================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // =========================
            // Unit Of Work
            // =========================
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // =========================
            // Identity
            // =========================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // =========================
            // AutoMapper
            // =========================

            builder.Services.AddAutoMapper(
                typeof(MappingProfile).Assembly,
                typeof(IdentityMappingProfile).Assembly
            );

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            // =========================
            // Controllers
            // =========================

            builder.Services.AddControllers();

            // =========================
            // Swagger
            // =========================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // =========================
            // Middleware
            // =========================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
            app.UseStaticFiles();

            app.Run();
        }
    }
}