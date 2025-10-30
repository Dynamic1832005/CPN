// File: Models/JobViewModel.cs
using System;

namespace CPN.Models
{
    public class JobViewModel
    {
        public int Id { get; set; }               // Job ID
        public string Title { get; set; }         // Job title (e.g., Web Developer)
        public string Description { get; set; }   // Job description
        public string Department { get; set; }    // Department name (CS or CT)
        public string VideoUrl { get; set; }      // Optional - YouTube or local video link

        // Optional: date when job was added or last updated
        public DateTime? CreatedDate { get; set; }
    }
}
