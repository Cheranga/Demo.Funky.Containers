using System;
using System.Threading.Tasks;
using FunkyContainers.Core;
using FunkyContainers.Infrastructure.CustomerApi;
using FunkyContainers.Infrastructure.DataAccess;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunkyContainers.Features.ConfirmReservation
{
    public class ConfirmReservationFunction
    {
        private readonly IConfirmReservationService _service;
        private readonly ILogger<ConfirmReservationFunction> _logger;

        public ConfirmReservationFunction(IConfirmReservationService service, ILogger<ConfirmReservationFunction> logger)
        {
            _service = service;
            _logger = logger;
        }

        [FunctionName(nameof(ConfirmReservationFunction))]
        public async Task RunAsync([QueueTrigger("%reservationrequests%")] ReserveHotelMessage message, 
            [Queue("%reservationconfirmations%")] IAsyncCollector<ConfirmedReservationMessage> messages)
        {
            _logger.LogInformation("{CorrelationId}: reservation request received", message.CorrelationId);

            var operation = await _service.ConfirmAsync(message);
            if (!operation.Status)
            {
                _logger.LogError("{CorrelationId}: error occurred when confirming", message.CorrelationId);
                return;
            }

            var publishOperation = await PublishMessageAsync(message, operation.Data, messages);
            if (!publishOperation.Status)
            {
                _logger.LogError("{CorrelationId}: error when publishing message", message.CorrelationId);
            }
        }

        private async Task<Result> PublishMessageAsync(ReserveHotelMessage message, GetCustomerResponse data, IAsyncCollector<ConfirmedReservationMessage> messages)
        {
            var confirmedMessage = new ConfirmedReservationMessage
            {
                CorrelationId = message.CorrelationId,
                ReservationId = message.ReservationId,
                CustomerData = data
            };

            await messages.AddAsync(confirmedMessage);

            return Result.Success();
        }
    }
}