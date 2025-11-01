using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiApi.Models
{
    [Table("rate_codes")]
    public class RateCode
    {
        [Key]
        [Column("rate_code_id")]
        public int RateCodeId { get; set; }

        [Column("rate_description")]
        [StringLength(50)]
        public string RateDescription { get; set; } = string.Empty;

        public ICollection<YellowTripData>? Trips { get; set; }
    }
}
