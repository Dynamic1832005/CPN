using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPN.Models
{
    public class CTJobQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Job")]
        public int JobId { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string OptionA { get; set; }

        [Required]
        public string OptionB { get; set; }

        [Required]
        public string OptionC { get; set; }

        [Required]
        public string OptionD { get; set; }

        [Required]
        public string CorrectAnswer { get; set; }

        // Navigation property
        public virtual CTJob Job { get; set; }
    }
}
