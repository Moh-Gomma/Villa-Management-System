using Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Update(Booking entity);
        void UpdateStatus(int  BookingID , string OrderStatus);
        void UpdatePaymentStatus(int BookingID, string sessionId , string PaymentIntentId);
    }
}
