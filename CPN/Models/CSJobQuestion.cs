using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPN.Models
{
    public class CSJobQuestion
    {
        public int Id { get; set; }

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

        public virtual CSJob Job { get; set; }
    }
}

