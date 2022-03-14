using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using FluentValidation;
using FunkyContainers.Extensions;
using FunkyContainers.Features.ConfirmReservation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunkyContainers.Features.ReserveHotel
{
    public class ReserveHotelFunction
    {
        private readonly IValidator<ReserveHotelRequest> _validator;
        private readonly ILogger<ReserveHotelFunction> _logger;

        public ReserveHotelFunction(IValidator<ReserveHotelRequest> validator, ILogger<ReserveHotelFunction> logger)
        {
            _validator = validator;
            _logger = logger;
        }

        [FunctionName(nameof(ReserveHotelFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "hotels/reservations")] HttpRequest req,
            [Queue("%reservationrequests%")] IAsyncCollector<ReserveHotelMessage> messages)
        {
            var reserveHotelRequest = await req.ToModel<ReserveHotelRequest>();
            var validationResult = await _validator.ValidateAsync(reserveHotelRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("invalid request received");
                return new BadRequestObjectResult(validationResult);
            }

            var status = await PublishMessageAsync(messages, reserveHotelRequest);
            if (!status)
            {
                return new ObjectResult("internal server error")
                {
                    StatusCode = (int) (HttpStatusCode.InternalServerError)
                };
            }

            return new AcceptedResult();
        }

        private async Task<bool> PublishMessageAsync(IAsyncCollector<ReserveHotelMessage> messages, ReserveHotelRequest request)
        {
            var message = new ReserveHotelMessage
            {
                CorrelationId = request.CorrelationId,
                ReservationId = request.ReservationId,
                UserId = request.UserId
            };

            await messages.AddAsync(message);
            return true;
        }
    }
}