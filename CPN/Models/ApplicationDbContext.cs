using System.Data.Entity;

namespace CPN.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=CPNConnection") { }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<CSJob> CSJobs { get; set; }
        public DbSet<CSJobQuestion> CSJobQuestions { get; set; }
        public DbSet<CTJob> CTJobs { get; set; }
        public DbSet<CTJobQuestion> CTJobQuestions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }



    }
}

