using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPN.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^[A-Za-z].*$", ErrorMessage = "Username must start with a letter")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Password must be 5 to 10 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public string Role { get; set; }
        public string Session { get; set; }
        public virtual ICollection<QuizResult> QuizResults { get; set; }
    }
}
