using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class Client
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        public string FullName {
            get => $"{FirstName} {LastName}";
        }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}
