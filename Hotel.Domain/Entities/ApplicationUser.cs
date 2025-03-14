using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = " The First name is Required")]
        [StringLength(50 , ErrorMessage = " Name is Too Long") , MinLength(3 , ErrorMessage = " Name is too Short")]
        public string FirstName { get; set; } =string.Empty;
        [Required(ErrorMessage = " The Last name is Required")]
        [StringLength(50, ErrorMessage = " Name is Too Long"), MinLength(3, ErrorMessage = " Name is too Short")]
        public string LastName { get; set; } =string.Empty;
        public DateTime CreationTime { get; set; }
        public string? PersonalId { get; set; }

    }
}
