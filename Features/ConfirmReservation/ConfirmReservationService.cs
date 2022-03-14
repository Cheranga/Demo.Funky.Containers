using System.Threading.Tasks;
using FunkyContainers.Core;
using FunkyContainers.Infrastructure.CustomerApi;
using FunkyContainers.Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;

namespace FunkyContainers.Features.ConfirmReservation
{
    public interface IConfirmReservationService
    {
        Task<Result<GetCustomerResponse>> ConfirmAsync(ReserveHotelMessage message);
    }
    
    public class ConfirmReservationService : IConfirmReservationService
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly ICommandHandler<SaveReservationCommand> _commandHandler;
        private readonly ILogger<ConfirmReservationService> _logger;

        public ConfirmReservationService(ICustomerApiService customerApiService, ICommandHandler<SaveReservationCommand> commandHandler, ILogger<ConfirmReservationService> logger)
        {
            _customerApiService = customerApiService;
            _commandHandler = commandHandler;
            _logger = logger;
        }
        
        public async Task<Result<GetCustomerResponse>> ConfirmAsync(ReserveHotelMessage message)
        {
            var getCustomerOperation = await GetCustomerDataAsync(message);
            if (!getCustomerOperation.Status)
            {
                _logger.LogError("{CorrelationId}: error when getting customer data", message.CorrelationId);
                return Result<GetCustomerResponse>.Failure(getCustomerOperation.ErrorCode, getCustomerOperation.ErrorMessage);
            }

            var saveOperation = await SaveReservationDataAsync(message, ReservationStatus.Confirmed, getCustomerOperation.Data);
            if (!saveOperation.Status)
            {
                _logger.LogError("{CorrelationId}: error when saving reservation data", message.CorrelationId);
                return Result<GetCustomerResponse>.Failure(saveOperation.ErrorCode, saveOperation.ErrorMessage);
            }

            return getCustomerOperation;
        }
        
        private async Task<Result<GetCustomerResponse>> GetCustomerDataAsync(ReserveHotelMessage message)
        {
            var customerId = message.UserId % 11;
            var response = await _customerApiService.GetCustomerAsync(new GetCustomerRequest
            {
                CorrelationId = message.CorrelationId,
                CustomerId = customerId.ToString()
            });

            return response;
        }
        
        private async Task<Result> SaveReservationDataAsync(ReserveHotelMessage message, ReservationStatus status, GetCustomerResponse response)
        {
            var command = new SaveReservationCommand
            {
                CorrelationId = message.CorrelationId,
                ReservationId = message.ReservationId,
                Name = response.Name,
                Email = response.Email,
                UserName = response.UserName,
                City = response.Address?.City,
                Street = response.Address?.Street,
                Suite = response.Address?.Suite,
                ZipCode = response.Address?.ZipCode,
                Status = status
            };

            var operation = await _commandHandler.ExecuteAsync(command);
            return operation;
        }
    }
}