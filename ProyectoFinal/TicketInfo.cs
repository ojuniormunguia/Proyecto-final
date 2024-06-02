using System.Windows.Media.Imaging;

namespace ProyectoFinal
{
    public class TicketInfo
    {
        public string ClientName { get; set; }
        public string ScheduleTime { get; set; }
        public string SeatNumber { get; set; }
        public string TicketCode { get; set; }
        public BitmapImage QRCodeImage { get; set; }
    }
}
