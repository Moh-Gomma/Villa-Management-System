using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Hotel.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWOrk;

        public BookingController(IUnitOfWork unitOfWOrk)
        {
            _unitOfWOrk = unitOfWOrk;
        }
        [Authorize]
        public IActionResult FinalizeBooking(int villaId, DateTime checkInDate,
            int Nights)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWOrk.User.Get(x => x.Id == UserId);

            if (!IsAvailableForDateRange(villaId, checkInDate, checkInDate.AddDays(Nights)))
            {
                ModelState.AddModelError("", " Villa is not available for the selected dates.");
                return View(new Booking());
            }

            Booking booking = new Booking()
            {
                VillaId = villaId,
                CheckInDate = checkInDate,

                Nights = Nights
                ,
                Villa = _unitOfWOrk.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
                CheckOutDate = checkInDate.AddDays(Nights),
                UserId = UserId,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            booking.TotalCost = booking.Villa.Price * Nights;

            return View(booking);
        }


        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWOrk.Villa.Get(u => u.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;

            booking.BookingNumber = GenerateUniqueBookNumber(); // Unique

            booking.status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            (_unitOfWOrk.Booking as BookingRepository)?.UpdateVillaAvailability(booking.VillaId, booking.CheckInDate, booking.CheckOutDate, true);

            _unitOfWOrk.Booking.Add(booking);
            _unitOfWOrk.Save();


            var successUrl = Url.Action("BookingConfirmation", "Booking", new { bookingId = booking.Id }, Request.Scheme, Request.Host.Value);
            
            var cancelUrl = Url.Action("FinalizeBooking", "Booking", new { villaId = booking.VillaId, checkInDate = booking.CheckInDate.ToString("yyyy-MM-dd"), nights = booking.Nights }, Request.Scheme, Request.Host.Value);


            var imageUrl = villa.ImageUrl;
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                imageUrl = $"{Request.Scheme}://{Request.Host.Value}{villa.ImageUrl}";
            }

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string> { { "bookingId", booking.Id.ToString() } }
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                    }
                },
                Quantity = 1
            });

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWOrk.Booking.UpdatePaymentStatus(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWOrk.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingFromDb = _unitOfWOrk.Booking.Get(b => b.Id == bookingId, includeProperties: "User,Villa");
            if (bookingFromDb == null)
            {
                return NotFound("Booking not found.");
            }

            if (bookingFromDb.status == SD.StatusPending)
            {
                var service = new SessionService();
                try
                {

                    Session session = service.Get(bookingFromDb.StripeSessionId);
                    if (session.PaymentStatus == "paid")
                    {
                        _unitOfWOrk.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved);
                        _unitOfWOrk.Booking.UpdatePaymentStatus(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                        _unitOfWOrk.Save();
                    }
                }
                catch (Exception ex)
                {
                    return View("Error", new { message = "Error processing payment status: " + ex.Message });
                }
            }

            return View(bookingFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelBooking(int bookingId)
        {
            var booking = _unitOfWOrk.Booking.Get(b => b.Id == bookingId, includeProperties: "Villa");
            var response = new { success = false, message = "An error occurred." };

            if (booking != null && booking.UserId == User.FindFirst(ClaimTypes.NameIdentifier)?.Value &&
                booking.status != SD.StatusCompleted && booking.status != SD.StatusCancelled)
            {
                booking.status = SD.StatusCancelled;
                _unitOfWOrk.Booking.Update(booking);
                (_unitOfWOrk.Booking as BookingRepository)?.UpdateVillaAvailability(booking.VillaId, booking.CheckInDate, booking.CheckOutDate, false);
                _unitOfWOrk.Save();
                response = new { success = true, message = "Booking canceled successfully." };
            }
            else
            {
                response = new { success = false, message = "Unauthorized or invalid booking status." };
            }

            return Json(response);
        }


        public IActionResult BookingHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var booking = _unitOfWOrk.Booking.GetAll(b => b.UserId == userId, includeProperties: "Villa")
                            .OrderByDescending(b => b.CheckInDate).ToList();
            return View(booking);
        }

        private string GenerateUniqueBookNumber()
        {
            var now = DateTime.Now;
            string prefix = "BK";
            string dataPart = now.ToString("yyMMdd");
            string randomPart = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            string bookingNumber = $"{prefix}{dataPart}-{randomPart}";

            while (_unitOfWOrk.Booking.Any(b => b.BookingNumber == bookingNumber))
            {
                randomPart = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
                bookingNumber = $"{prefix}{dataPart}-{randomPart}";
            }
            return bookingNumber;
        }


        private bool IsAvailableForDateRange(int villaId, DateTime checkInDate, DateTime checkOutDate)
        {
            var overlappingBookings = _unitOfWOrk.Booking.GetAll(
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
