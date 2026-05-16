using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public string NameAr { get; set; }
        public string NameEn { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
    public class CreateCategoryDto
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }


    public class UpdateCategoryDto
    {
        public Guid Id { get; set; }

        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string? Description { get; set; }

        public IFormFile? Image { get; set; }

        public bool IsActive { get; set; }
    }
}
