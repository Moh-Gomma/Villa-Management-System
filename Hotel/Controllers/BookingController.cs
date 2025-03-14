using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
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

        //[Authorize]
        //[HttpPost]
        //public IActionResult FinalizeBooking(Booking booking)
        //{
        //    var villa = _unitOfWOrk.Villa.Get(u => u.Id == booking.VillaId);
        //    booking.TotalCost = villa.Price * booking.Nights;

        //    booking.BookingNumber = GenerateUniqueBookNumber();// Unique

        //    booking.status = SD.StatusPending;
        //    booking.BookingDate = DateTime.Now;

        //    _unitOfWOrk.Booking.Add(booking);
        //    _unitOfWOrk.Save();
        //    // Generate fully qualified URLs using Url.Action
        //    var successUrl = Url.Action("BookingConfirmation", "Booking", new { bookingId = booking.Id }, Request.Scheme, Request.Host.Value);
        //    var cancelUrl = Url.Action("FinalizeBooking", "Booking", new { villaId = booking.VillaId, checkInDate = booking.CheckInDate.ToString("yyyy-MM-dd"), nights = booking.Nights }, Request.Scheme, Request.Host.Value);

        //    var domain = Request.Scheme + ":/" + Request.Host.Value;
        //    var options = new SessionCreateOptions
        //    {
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //        SuccessUrl = successUrl,
        //        CancelUrl = cancelUrl,
        //    };

        //    options.LineItems.Add(new SessionLineItemOptions
        //    {
        //        PriceData = new SessionLineItemPriceDataOptions
        //        {
        //            UnitAmount = (long)(booking.TotalCost * 100),
        //            Currency = "usd",
        //            ProductData = new SessionLineItemPriceDataProductDataOptions
        //            {
        //                Name = villa.Name,
        //                //Images = new List<string> { domain + villa.ImageUrl }
        //            }
        //        },
        //        Quantity = 1
        //    });

        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    _unitOfWOrk.Booking.UpdatePaymentStatus(booking.Id, session.Id, session.PaymentIntentId);
        //    _unitOfWOrk.Save();

        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);

        //    //return RedirectToAction(nameof(BookingConfirmation), new { bookingId = booking.Id });
        //}
        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWOrk.Villa.Get(u => u.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;

            booking.BookingNumber = GenerateUniqueBookNumber(); // Unique

            booking.status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            _unitOfWOrk.Booking.Add(booking);
            _unitOfWOrk.Save();

            // Generate fully qualified URLs using Url.Action
            var successUrl = Url.Action("BookingConfirmation", "Booking", new { bookingId = booking.Id }, Request.Scheme, Request.Host.Value);
            var cancelUrl = Url.Action("FinalizeBooking", "Booking", new { villaId = booking.VillaId, checkInDate = booking.CheckInDate.ToString("yyyy-MM-dd"), nights = booking.Nights }, Request.Scheme, Request.Host.Value);

            // Ensure the ImageUrl is a valid URL
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
            Booking bookingFromDb = _unitOfWOrk.Booking.Get(b => b.Id == bookingId , includeProperties: "User,Villa");
            if(bookingFromDb.status == SD.StatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(bookingFromDb.StripeSessionId);
                if(session.PaymentStatus == "paid")
                {
                    _unitOfWOrk.Booking.UpdateStatus(bookingFromDb.Id, SD.StatusApproved);
                    _unitOfWOrk.Booking.UpdatePaymentStatus(bookingFromDb.Id, session.Id, session.PaymentIntentId);
                    _unitOfWOrk.Save();
                }
            }
 
            return View(bookingId);
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
    }
}
