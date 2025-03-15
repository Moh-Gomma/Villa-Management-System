using Hotel.Domain.Entities;

namespace Hotel.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalBaoking { get; set; }
        public double TotalRevenue { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public List<int> BookingTrends { get; set; }
        public List<string> BookingTrendLabels { get; set; }
        public List<Booking> Bookings { get; set; } 


    }
}
