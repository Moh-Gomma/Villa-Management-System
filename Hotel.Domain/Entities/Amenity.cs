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
    public class Amenity
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is Required")]
        [StringLength(50 , ErrorMessage = " Max Length is 50 Character") , MinLength(3 , ErrorMessage = " name is too Short")]
        
        public string Name { get; set; }
        [StringLength(500, ErrorMessage = " Too Long Text"), MinLength(3, ErrorMessage = " Description is too Short")]

        public string? Description { get; set; }

        //navigation Properity
        [ForeignKey("villa")]
        public int VillaId { get; set; }
        [ValidateNever]
        public Villa villa { get; set; }

    }
}
