using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace FunkyContainers.Extensions
{
    public static class HttpExtensions
    {
        public static async Task<TData> ToModel<TData>(this HttpRequest request) where TData : class
        {
            var content = await request.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            var model = JsonConvert.DeserializeObject<TData>(content, new JsonSerializerSettings
            {
                Error = (sender, args) => args.ErrorContext.Handled = true
            });

            return model;
        }
    }
}