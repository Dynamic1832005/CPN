// File: Models/CTJob.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPN.Models
{
    public class CTJob
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        // Optional: if you're using questions
        public virtual ICollection<CTJobQuestion> Questions { get; set; }
    }
}
