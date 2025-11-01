using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiApi.Models
{
    [Table("yellow_tripdata")]
    public class YellowTripData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Column("VendorID")]
        public int VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor? Vendor { get; set; }

        [Column("tpep_pickup_datetime")]
        public DateTime PickupDatetime { get; set; }

        [Column("tpep_dropoff_datetime")]
        public DateTime DropoffDatetime { get; set; }

        [Column("passenger_count")]
        public int? PassengerCount { get; set; }

        [Column("trip_distance")]
        public decimal TripDistance { get; set; }

        [Column("RatecodeID")]
        public int RateCodeId { get; set; }

        [ForeignKey("RateCodeId")]
        public RateCode? RateCode { get; set; }

        [Column("store_and_fwd_flag")]
        [StringLength(1)]
        public string? StoreAndFwdFlag { get; set; }

        [Column("PULocationID")]
        public int PULocationId { get; set; }

        [ForeignKey("PULocationId")]
        public TaxiZone? PickupZone { get; set; }

        [Column("DOLocationID")]
        public int DOLocationId { get; set; }

        [ForeignKey("DOLocationId")]
        public TaxiZone? DropoffZone { get; set; }

        [Column("payment_type")]
        public int PaymentTypeId { get; set; }

        [ForeignKey("PaymentTypeId")]
        public PaymentType? PaymentType { get; set; }

        [Column("fare_amount")]
        public decimal FareAmount { get; set; }

        [Column("extra")]
        public decimal? Extra { get; set; }

        [Column("mta_tax")]
        public decimal? MtaTax { get; set; }

        [Column("tip_amount")]
        public decimal? TipAmount { get; set; }

        [Column("tolls_amount")]
        public decimal? TollsAmount { get; set; }

        [Column("improvement_surcharge")]
        public decimal? ImprovementSurcharge { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("congestion_surcharge")]
        public decimal? CongestionSurcharge { get; set; }

        [Column("Airport_fee")]
        public decimal? AirportFee { get; set; }

        [Column("cbd_congestion_fee")]
        public decimal? CbdCongestionFee { get; set; }
    }
}
