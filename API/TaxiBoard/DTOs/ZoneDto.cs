namespace TaxiBoard.DTOs
{
    public class ZoneDto
    {
        public int LocationId { get; set; }
        public string Borough { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public string ServiceZone { get; set; } = string.Empty;
    }
}
