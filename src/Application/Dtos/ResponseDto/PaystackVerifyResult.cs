using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ResponseDto
{
    public class PaystackVerifyResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Reference { get; set; } = default!;
        public string GatewayResponse { get; set; } = default!;
    }
}
