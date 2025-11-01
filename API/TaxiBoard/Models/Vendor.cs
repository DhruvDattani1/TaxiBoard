using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiApi.Models
{
    [Table("vendors")]
    public class Vendor
    {
        [Key]
        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Column("vendor_name")]
        [StringLength(100)]
        public string VendorName { get; set; } = string.Empty;

        public ICollection<YellowTripData>? Trips { get; set; }
    }
}
