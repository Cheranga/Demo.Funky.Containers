namespace FunkyContainers.Features.ConfirmReservation
{
    public class ReserveHotelMessage
    {
        public string CorrelationId { get; set; }
        public string ReservationId { get; set; }
        public int UserId { get; set; }
    }
}