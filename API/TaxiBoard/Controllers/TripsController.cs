using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBoard.Data;
using TaxiBoard.DTOs;

namespace TaxiBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly TaxiBoardContext _context;

        public TripsController(TaxiBoardContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TripDto>>> GetTrips(
            int page = 1,
            int pageSize = 50,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? passengers = null)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Invalid pagination values.");

            var query = _context.YellowTripData
                .Include(t => t.PickupZone)
                .Include(t => t.DropoffZone)
                .Include(t => t.PaymentType)
                .Include(t => t.Vendor)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.PickupDatetime >= DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc));
            if (endDate.HasValue)
                query = query.Where(t => t.PickupDatetime <= DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc));
            if (passengers.HasValue)
                query = query.Where(t => t.PassengerCount == passengers.Value);

            var totalCount = await query.CountAsync();

            var trips = await query
                .OrderByDescending(t => t.PickupDatetime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    PickupDatetime = t.PickupDatetime,
                    DropoffDatetime = t.DropoffDatetime,
                    PassengerCount = t.PassengerCount ?? 0,
                    TripDistance = t.TripDistance,
                    PickupZone = t.PickupZone != null ? t.PickupZone.Zone : "Unknown",
                    DropoffZone = t.DropoffZone != null ? t.DropoffZone.Zone : "Unknown",
                    TotalAmount = t.TotalAmount,
                    PaymentType = t.PaymentType != null ? t.PaymentType.PaymentDescription : "Unknown",
                    VendorName = t.Vendor != null ? t.Vendor.VendorName : "Unknown"
                })
                .ToListAsync();

            var result = new PagedResultDto<TripDto>
            {
                Data = trips,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(result);
        }
    }
}
