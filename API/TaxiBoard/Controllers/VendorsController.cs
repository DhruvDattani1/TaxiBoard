using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBoard.Data;
using TaxiBoard.DTOs;

namespace TaxiBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly TaxiBoardContext _context;

        public VendorsController(TaxiBoardContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendors()
        {
            var result = await _context.Vendors
                .AsNoTracking()
                .OrderBy(v => v.VendorId)
                .Select(v => new VendorDto
                {
                    VendorId = v.VendorId,
                    VendorName = v.VendorName
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
