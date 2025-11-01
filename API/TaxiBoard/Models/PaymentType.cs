using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiApi.Models
{
    [Table("payment_types")]
    public class PaymentType
    {
        [Key]
        [Column("payment_type_id")]
        public int PaymentTypeId { get; set; }

        [Column("payment_description")]
        [StringLength(50)]
        public string PaymentDescription { get; set; } = string.Empty;

        public ICollection<YellowTripData>? Trips { get; set; }
    }
}

