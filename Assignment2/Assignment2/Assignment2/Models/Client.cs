using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        public string FullName { get => $"{FirstName} {LastName}"; }

        // Navigation props
        public virtual IList<Subscription> Subscriptions { get; set; }
    }
}
