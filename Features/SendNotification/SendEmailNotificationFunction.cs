using System;
using System.Threading.Tasks;
using FunkyContainers.Core;
using FunkyContainers.Features.ConfirmReservation;
using FunkyContainers.Infrastructure.CustomerApi;
using FunkyContainers.Infrastructure.DataAccess;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunkyContainers.Features.SendNotification
{
    public class SendEmailNotificationFunction
    {
        private readonly ICommandHandler<SaveReservationCommand> _commandHandler;
        private readonly ILogger<SendEmailNotificationFunction> _logger;

        public SendEmailNotificationFunction(ICommandHandler<SaveReservationCommand> commandHandler, ILogger<SendEmailNotificationFunction> logger)
        {
            _commandHandler = commandHandler;
            _logger = logger;
        }
        
        [FunctionName(nameof(SendEmailNotificationFunction))]
        public async Task RunAsync([QueueTrigger("%reservationconfirmations%")] ConfirmedReservationMessage message)
        {
            // TODO: Use the email service to send an email
            await Task.Delay(TimeSpan.FromSeconds(2));

            await SaveReservationDataAsync(message, ReservationStatus.Notified);
        }
        
        private async Task<Result> SaveReservationDataAsync(ConfirmedReservationMessage message, ReservationStatus status)
        {
            var customerData = message.CustomerData;
            var command = new SaveReservationCommand
            {
                CorrelationId = message.CorrelationId,
                ReservationId = message.ReservationId,
                Name = customerData.Name,
                Email = customerData.Email,
                UserName = customerData.UserName,
                City = customerData.Address?.City,
                Street = customerData.Address?.Street,
                Suite = customerData.Address?.Suite,
                ZipCode = customerData.Address?.ZipCode,
                Status = status
            };

            var operation = await _commandHandler.ExecuteAsync(command);
            return operation;
        }
    }
}