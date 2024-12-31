using System.ComponentModel.DataAnnotations; 

namespace Lab5.Models
{
    public enum Question { Earth, Computer };

    public class AnswerImage
    {
        public int AnswerImageId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string FileName { get; set; }
        [Required]
        [Url]
        public string Url { get; set; }
        [Required]
        public Question Question { get; set; }
    }
}
