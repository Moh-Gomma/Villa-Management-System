using Hotel.Application.Common.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructue.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Booking entity)
        {
            _db.Bookings.Update(entity);
        }

        public void UpdatePaymentStatus(int BookingID, string sessionId, string PaymentIntentId)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(b => b.Id == BookingID);
            if (bookingFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    bookingFromDb.StripeSessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(PaymentIntentId))
                {
                    bookingFromDb.StripePaymentIntentId = PaymentIntentId;
                    bookingFromDb.PaymentDate = DateTime.Now;
                    bookingFromDb.IsPayMentSuccessful = true;
                }
            }
        }

        public void UpdateStatus(int BookingID, string OrderStatus)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(b=>b.Id == BookingID);
            if (bookingFromDb != null)
            {
                bookingFromDb.status = OrderStatus;
                if(OrderStatus == SD.StatusCheckIn)
                {
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                }
                if (OrderStatus == SD.StatusCompleted)
                {
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
                }
            }
            
        }
    }
}
