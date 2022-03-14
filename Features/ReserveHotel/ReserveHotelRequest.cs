namespace FunkyContainers.Features.ReserveHotel
{
    public class ReserveHotelRequest
    {
        public string CorrelationId { get; set; }
        public string ReservationId { get; set; }
        public int UserId { get; set; }
    }
}