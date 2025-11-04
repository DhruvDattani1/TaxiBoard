namespace TaxiBoard.DTOs
{
    public class TripAnalyticsDto
    {
        public int TotalTrips { get; set; }
        public decimal AverageFare { get; set; }
        public decimal AverageDistance { get; set; }
        public decimal TotalRevenue { get; set; }
        public string MostPopularPickupZone { get; set; } = string.Empty;
        public string MostUsedPaymentType { get; set; } = string.Empty;
    }
}
