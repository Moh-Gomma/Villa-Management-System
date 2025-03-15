using System.Diagnostics;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Models;
using Hotel.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var homeVm = new HomeVM()
            {
                VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
                Nights = 1,
                CheckInDate = DateTime.Now.AddDays(1),
                IsAvailabilityChecked = false
            };
            foreach (var vill in homeVm.VillaList)
            {
                if (vill.Id % 2 == 0)
                {
                    vill.IsAvailable = false;
                }
            }
            return View(homeVm);
        }


        [HttpPost]
        public IActionResult GetVillaById([FromForm] int Nights , [FromForm] DateTime CheckInDate)
        {
            Thread.Sleep(1000); // Simulate delay

            var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");

            HomeVM model = new HomeVM()
            {
                CheckInDate = CheckInDate,
                Nights = Nights,
                VillaList = villaList ,
                IsAvailabilityChecked = true
            };
            //
            foreach(var vill in villaList)
            {
                vill.IsAvailable = IsAvailableForDateRange(vill.Id, CheckInDate, CheckInDate.AddDays(Nights));
            }

            return PartialView("_VillaListHome",model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private bool IsAvailableForDateRange(int villaId, DateTime checkInDate, DateTime checkOutDate)
        {
            var overlappingBookings = _unitOfWork.Booking.GetAll(
                b => b.VillaId == villaId &&
                     b.status != SD.StatusCancelled &&
                     b.status != SD.StatusCompleted &&
                     ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
                      (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate) ||
                      (checkInDate <= b.CheckInDate && checkOutDate >= b.CheckOutDate)),
                includeProperties: "Villa"
            );

            return !overlappingBookings.Any();
        }
    }
}
