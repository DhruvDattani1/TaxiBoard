namespace TaxiBoard.DTOs
{
    public class TripDto
    {
        public int Id { get; set; }
        public DateTime PickupDatetime { get; set; }
        public DateTime DropoffDatetime { get; set; }
        public int PassengerCount { get; set; }
        public decimal TripDistance { get; set; }
        public string PickupZone { get; set; } = string.Empty;
        public string DropoffZone { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
    }
}
