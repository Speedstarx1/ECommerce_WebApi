using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ResponseDto
{
    public class CheckoutResponseDto
    {
        public OrderDto Order { get; set; } = default!;
        public string AuthorizationUrl { get; set; } = default!;
        public string PaystackReference { get; set; } = default!;
    }
}
