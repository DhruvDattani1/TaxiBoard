using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiBoard.Data;
using TaxiBoard.DTOs;

namespace TaxiBoard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly TaxiBoardContext _context;

        public AnalyticsController(TaxiBoardContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<TripAnalyticsDto>> GetTripAnalytics(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.YellowTripData
                .Include(t => t.PickupZone)
                .Include(t => t.PaymentType)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.PickupDatetime >= DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc));

            if (endDate.HasValue)
                query = query.Where(t => t.PickupDatetime <= DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc));

            var totalTrips = await query.CountAsync();

            if (totalTrips == 0)
            {
                return Ok(new TripAnalyticsDto
                {
                    TotalTrips = 0,
                    AverageFare = 0,
                    AverageDistance = 0,
                    TotalRevenue = 0,
                    MostPopularPickupZone = "No Data",
                    MostUsedPaymentType = "No Data"
                });
            }

            var averageFare = await query.AverageAsync(t => t.TotalAmount);
            var averageDistance = await query.AverageAsync(t => t.TripDistance);
            var totalRevenue = await query.SumAsync(t => t.TotalAmount);

            var mostPopularPickupZone = await query
                .Where(t => t.PickupZone != null)
                .GroupBy(t => t.PickupZone.Zone)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync() ?? "Unknown";

            var mostUsedPaymentType = await query
                .Where(t => t.PaymentType != null)
                .GroupBy(t => t.PaymentType.PaymentDescription)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync() ?? "Unknown";

            var result = new TripAnalyticsDto
            {
                TotalTrips = totalTrips,
                AverageFare = Math.Round(averageFare, 2),
                AverageDistance = Math.Round(averageDistance, 2),
                TotalRevenue = Math.Round(totalRevenue, 2),
                MostPopularPickupZone = mostPopularPickupZone,
                MostUsedPaymentType = mostUsedPaymentType
            };

            return Ok(result);
        }
    }
}
