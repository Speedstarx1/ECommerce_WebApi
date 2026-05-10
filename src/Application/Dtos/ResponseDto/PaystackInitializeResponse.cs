namespace Application.Dtos.ResponseDto
{
    public class PaystackInitializeResponse
    {
        public string AuthorizationUrl { get; set; } = default!;
        public string AccessCode { get; set; } = default!;
        public string Reference { get; set; } = default!;
    }

    
}