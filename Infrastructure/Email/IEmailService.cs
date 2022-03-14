using System;
using System.Threading.Tasks;
using FunkyContainers.Core;
using Microsoft.Extensions.Logging;

namespace FunkyContainers.Infrastructure.Email
{
    public interface IEmailService
    {
        Task<Result<SendCancelConfirmationEmailResponse>> SendEmailAsync(SendCancelConfirmationEmailRequest request);
    }
    
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }
        
        public Task<Result<SendCancelConfirmationEmailResponse>> SendEmailAsync(SendCancelConfirmationEmailRequest request)
        {
            _logger.LogInformation("{CorrelationId}: Email sent successfully", request.CorrelationId);
            return Task.FromResult(Result<SendCancelConfirmationEmailResponse>.Success(new SendCancelConfirmationEmailResponse
            {
                TrackingId = $"{request.CorrelationId}-{DateTime.Now:HHmmss}"
            }));
        }
    }
}