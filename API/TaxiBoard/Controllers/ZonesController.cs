using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBoard.Data;
using TaxiBoard.DTOs;

namespace TaxiBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController : ControllerBase
    {
        private readonly TaxiBoardContext _context;

        public ZonesController(TaxiBoardContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZoneDto>>> GetZones(string? borough = null)
        {
            var query = _context.TaxiZones.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(borough))
                query = query.Where(z => z.Borough.ToLower() == borough.ToLower());

            var result = await query
                .OrderBy(z => z.Zone)
                .Select(z => new ZoneDto
                {
                    LocationId = z.LocationId,
                    Borough = z.Borough,
                    Zone = z.Zone,
                    ServiceZone = z.ServiceZone
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}

