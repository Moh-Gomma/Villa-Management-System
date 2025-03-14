using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
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


            var model = new AdminDashboardViewModel
            {
                TotalBaoking = totalBooking,
                TotalRevenue = totalRevenue,
                PendingBookings = pendingBookings,
                ConfirmedBookings = ConfirmedBooking,
                BookingTrends = bookingTrends.Select(x => x.Count).ToList(),
                BookingTrendLabels = bookingTrends.Select(x => x.Month).ToList()
            };

            return View(model);
        }
    }
}
