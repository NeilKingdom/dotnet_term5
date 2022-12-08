using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class Advertisement
    {
        public int AdvertisementId { get; set; }

        [ForeignKey("BrokerageId")]
        public string BrokerageId { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string AdUrl { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        // Navigation props
        public virtual IList<Brokerage> Brokerages { get; set; }
    }
}
