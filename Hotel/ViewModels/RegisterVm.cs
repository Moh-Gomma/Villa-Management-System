using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Web.ViewModels
{
    public class RegisterVm
    {
        [Required(ErrorMessage = " The First name is Required")]
        [StringLength(50, ErrorMessage = " Name is Too Long"), MinLength(3, ErrorMessage = " Name is too Short")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = " The First name is Required")]
        [StringLength(50, ErrorMessage = " Name is Too Long"), MinLength(3, ErrorMessage = " Name is too Short")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password) , ErrorMessage = " Password Must Match")]
        [Display(Name = " Confirm Password")]

        public string ConfirmPassword { get; set; }
        [Display(Name = " Phone Number")]

        public string? PhoneNumber { get; set; }

        public string? RedirectUrl { get; set; }

        public string? Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? RoleList { get; set; }
    }
}
