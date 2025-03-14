using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public ApplicationUser User { get; set; }

        [Required]
        public int VillaId { get; set; }
        [ForeignKey("VillaId")]
        public Villa Villa { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = " Name is Too Long"), MinLength(3, ErrorMessage = " Name is too Short")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = " The Last name is Required")]
        [StringLength(50, ErrorMessage = " Name is Too Long"), MinLength(3, ErrorMessage = " Name is too Short")]
        public string LastName { get; set; } = string.Empty; [Required]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public double TotalCost { get; set; }
        public int Nights { get; set; }
        public string? status { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public bool IsPayMentSuccessful { get; set; }
        public DateTime PaymentDate { get; set; }

        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }

        public DateTime ActualCheckInDate { get; set; }
        public DateTime ActualCheckOutDate { get; set; }

        public int VillaNumber { get; set; }

        [StringLength(20)]
        public string? BookingNumber { get; set; }

    }
}
