using FluentValidation;
using FluentValidation.Results;
using FunkyContainers.Core;

namespace FunkyContainers.Features.ReserveHotel
{
    public class ReserveHotelRequestValidator : ModelValidatorBase<ReserveHotelRequest>
    {
        public ReserveHotelRequestValidator()
        {
            RuleFor(x => x.CorrelationId).NotNull().NotEmpty();
            RuleFor(x => x.ReservationId).NotNull().NotEmpty();
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}