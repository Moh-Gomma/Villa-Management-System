using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Repository;
using Hotel.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]

    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var totalBooking = _unitOfWork.Booking.GetAll().Count();
            var totalRevenue = _unitOfWork.Booking.GetAll().Sum(b => b.TotalCost);
            var pendingBookings = _unitOfWork.Booking.GetAll(b => b.status == SD.StatusPending).Count();
            var ConfirmedBooking = _unitOfWork.Booking.GetAll(b => b.status == SD.StatusApproved).Count();


            var now = DateTime.Now;
            var bookingTrends = Enumerable.Range(0, 6).Select(i =>
            {
                var month = now.AddMonths(-i);
                return new
                {
                    Month = month.ToString("MMM yyyy"),
                    Count = _unitOfWork.Booking.GetAll(b => b.BookingDate.Month == month.Month && b.BookingDate.Year == month.Year).Count()
                };
            }).OrderBy(x => DateTime.Parse(x.Month)).ToList();

            //  all bookings 
            var bookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Villa")
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            var model = new AdminDashboardViewModel
            {
                TotalBaoking = totalBooking,
                TotalRevenue = totalRevenue,
                PendingBookings = pendingBookings,
                ConfirmedBookings = ConfirmedBooking,
                BookingTrends = bookingTrends.Select(x => x.Count).ToList(),
                BookingTrendLabels = bookingTrends.Select(x => x.Month).ToList(),
                Bookings = bookings
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveBooking(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId);
            if (booking == null)
            {
                ViewData["Error"] = "Booking Not Found";
                return RedirectToAction("Index");
            }
            if (booking.status != SD.StatusPending)
            {
                TempData["Error"] = "Only pending bookings can be approved.";
                return RedirectToAction("Index");
            }
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusApproved);
            _unitOfWork.Save();

            TempData["Success"] = "Booking approved successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectBooking(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId, includeProperties: "Villa");
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Index");
            }

            if (booking.status != SD.StatusPending)
            {
                TempData["Error"] = "Only pending bookings can be rejected.";
                return RedirectToAction("Index");
            }

            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCancelled);
            // Make the villa available again
            (_unitOfWork.Booking as BookingRepository)?.UpdateVillaAvailability(
                booking.VillaId,
                booking.CheckInDate,
                booking.CheckOutDate,
                false 
            );

            _unitOfWork.Save();

            TempData["Success"] = "Booking rejected and villa made available.";
            return RedirectToAction("Index");

        }

        public IActionResult ManageBookings()
        {
            var bookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Villa")
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            return View(bookings);
        }
    }
}
