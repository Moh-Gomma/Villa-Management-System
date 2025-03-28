﻿using Hotel.Application.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int  Sqft  { get; set; }
        [Display(Name="Price Per Night")]
        [Range(1,10000)]
        public double Price { get; set; }
        [Range(1,10)]
        public int Occupancy { get; set; }
        [Display(Name ="Image Url")]
        public string? ImageUrl { get; set; }
        [NotMapped]
        [Display(Name ="Image")]
        [ImageValidation(maxSizeinMB: 5, new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile? Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ValidateNever]
        public IEnumerable<Amenity> VillaAmenity { get; set; }

        [NotMapped]
        public bool IsAvailable { get; set; } = true;


    }
}
