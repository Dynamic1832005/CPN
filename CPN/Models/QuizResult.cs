using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPN.Models
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }       // This exists in DB

        [Required]
        public string Department { get; set; }  // CS or CT

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public int Score { get; set; }

        [Required]
        public DateTime AttemptDate { get; set; }

        [ForeignKey("UserId")]          // <-- Tell EF to use UserId column
        public virtual AppUser AppUser { get; set; }
    }
}
