using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Attributes
{
    public class ImageValidationAttribute : ValidationAttribute
    {
        private readonly int  _MaxSizeinMB;
        private readonly string[] _allowedextention;

        public ImageValidationAttribute(int maxSizeinMB, string[] allowedextention)
        {
            _MaxSizeinMB = maxSizeinMB;
            _allowedextention = allowedextention;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return ValidationResult.Success;
            }
            if(value is not IFormFile image)
            {
                return new ValidationResult("Invalid Image");
            }
            var maxSizeInBytes = 5 * 1024 * 1024;
            if(image.Length > maxSizeInBytes)
            {
                return new ValidationResult("Image must be less than 5 MB");
            }
            var extention = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!_allowedextention.Contains(extention))
            {
                return new ValidationResult($"Only {string.Join(", ", _allowedextention)} files are allowed.");
            }

            return ValidationResult.Success;
        }
    }
}
