using Hotel.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Villa>? VillaList { get; set; }

        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        [Range(1, 30, ErrorMessage = "Number of nights must be between 1 and 30.")]
        public int Nights { get; set; }

        public bool IsAvailabilityChecked { get; set; }


    }
}
