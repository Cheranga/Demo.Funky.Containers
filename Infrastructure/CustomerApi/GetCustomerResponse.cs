namespace FunkyContainers.Infrastructure.CustomerApi
{
    public class GetCustomerResponse
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string Suite { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}