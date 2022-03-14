using System.Threading.Tasks;
using Azure.Data.Tables;
using FunkyContainers.Core;
using FunkyContainers.Infrastructure.DataAccess;
using Microsoft.Extensions.Logging;

namespace FunkyContainers.Features.ConfirmReservation
{
    public class SaveReservationCommandCommandHandler : ICommandHandler<SaveReservationCommand>
    {
        private const string CancelledReservationsTable = "reservations";
        private readonly TableServiceClient _serviceClient;
        private readonly ILogger<SaveReservationCommandCommandHandler> _logger;

        public SaveReservationCommandCommandHandler(TableServiceClient serviceClient, ILogger<SaveReservationCommandCommandHandler> logger)
        {
            _serviceClient = serviceClient;
            _logger = logger;
        }
        
        public async Task<Result> ExecuteAsync(SaveReservationCommand command)
        {
            var tableClient = _serviceClient.GetTableClient(CancelledReservationsTable);
            if (tableClient == null)
            {
                return Result.Failure(ErrorCodes.TableClientNotFound, ErrorMessages.TableClientNotFound);
            }

            var partitionKey = command.Status.ToString().ToUpper();
            var rowKey = command.ReservationId.ToUpper();
            var entity = new TableEntity(partitionKey, rowKey)
            {
                {nameof(command.CorrelationId), command.CorrelationId},
                {nameof(command.Name), command.Name},
                {nameof(command.City), command.City}
            };

            var response = await tableClient.UpsertEntityAsync(entity);
            if (response.IsError)
            {
                _logger.LogError("{CorrelationId} upsert failed because {FailedReason}", command.CorrelationId, response.ReasonPhrase);
                return Result.Failure(ErrorCodes.FailedUpsert, ErrorMessages.FailedUpsert);
            }

            return Result.Success();
        }
    }
}