using System;
using FunkyContainers.Infrastructure.CustomerApi;

namespace FunkyContainers.Features.ConfirmReservation
{
    public class ConfirmedReservationMessage
    {
        public string CorrelationId { get; set; }
        public string ReservationId { get; set; }

        public GetCustomerResponse CustomerData { get; set; }
    }
}