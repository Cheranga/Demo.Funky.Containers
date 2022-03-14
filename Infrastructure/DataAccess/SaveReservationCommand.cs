using FunkyContainers.Core;

namespace FunkyContainers.Infrastructure.DataAccess
{
    public class SaveReservationCommand : IOperation, ICommand
    {
        public string CorrelationId { get; set; }

        public string ReservationId { get; set; }

        public string TrackingId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Unconfirmed;
    }

    public enum ReservationStatus
    {
        Unconfirmed,
        Confirmed,
        Notified
    }
}