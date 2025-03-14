using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class VillaNumber
    {
        [Key , DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name ="Villa Number")]
        public int Villa_Number { get; set; }
        [ForeignKey("villa")]
        public int villaId { get; set; }
        [ValidateNever]
        public Villa villa { get; set; }
        public string? SpecialDetails { get; set; } 
    }
}
