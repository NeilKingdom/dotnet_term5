using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Lab6.Models
{
    public class Student
    {
        [SwaggerSchema(ReadOnly = true)]
        public Guid StudentId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Program")]
        [StringLength(5, MinimumLength = 1)]
        public string Program { get; set; }
    }
}
