using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiBoard.Models
{
    [Table("taxi_zones")]
    public class TaxiZone
    {
        [Key]
        [Column("locationid")]
        public int LocationId { get; set; }

        [Column("borough")]
        [StringLength(50)]
        public string Borough { get; set; } = string.Empty;

        [Column("zone")]
        [StringLength(100)]
        public string Zone { get; set; } = string.Empty;

        [Column("service_zone")]
        [StringLength(50)]
        public string ServiceZone { get; set; } = string.Empty;

        public ICollection<YellowTripData>? PickupTrips { get; set; }
        public ICollection<YellowTripData>? DropoffTrips { get; set; }
    }
}
