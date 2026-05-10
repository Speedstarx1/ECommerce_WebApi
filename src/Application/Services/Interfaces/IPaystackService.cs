using Application.Dtos.ResponseDto;

namespace Application.Services.Interfaces
{
    public interface IPaystackService
    {
        Task<PaystackInitializeResponse> InitializeTransactionAsync(string email, decimal amountNaira, string reference, string callbackUrl);
        Task<PaystackVerifyResult> VerifyTransactionAsync(string reference);
        bool VerifyWebhookSignature(string rawBody, string signature);
    }
}