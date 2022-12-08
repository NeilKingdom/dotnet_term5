using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class Brokerage
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registration Number")]
        public string BrokerageId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Fees")]
        [Range(0.00, (double)Decimal.MaxValue)]
        public decimal Fee { get; set; }

        // Navigation props
        public virtual IList<Subscription> Subscriptions { get; set; }
        public virtual IList<Advertisement> Advertisements { get; set; }
    }
}
