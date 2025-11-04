using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBoard.Data;
using TaxiBoard.DTOs;

namespace TaxiBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTypesController : ControllerBase
    {
        private readonly TaxiBoardContext _context;

        public PaymentTypesController(TaxiBoardContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentTypeDto>>> GetPaymentTypes()
        {
            var result = await _context.PaymentTypes
                .AsNoTracking()
                .OrderBy(p => p.PaymentTypeId)
                .Select(p => new PaymentTypeDto
                {
                    PaymentTypeId = p.PaymentTypeId,
                    PaymentDescription = p.PaymentDescription
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
