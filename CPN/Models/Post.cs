using System;
using System.ComponentModel.DataAnnotations;

namespace CPN.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string VideoPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}