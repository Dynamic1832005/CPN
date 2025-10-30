using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPN.Models
{
    public class CSJob
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        // Navigation property to related questions
        public virtual ICollection<CSJobQuestion> Questions { get; set; }
    }
}

