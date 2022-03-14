using FunkyContainers.Core;

namespace FunkyContainers.Infrastructure.CustomerApi
{
    public class GetCustomerRequest : IOperation
    {
        public string CustomerId { get; set; }
        public string CorrelationId { get; set; }
    }
}