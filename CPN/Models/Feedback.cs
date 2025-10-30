using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPN.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Name { get; set; }  // auto-filled
        public string Email { get; set; } // auto-filled
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
