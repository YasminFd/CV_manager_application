using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using proj.Models;


namespace proj.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//better ad for when adding other migrations
            modelBuilder.Entity<Skill>().HasData(new Skill
            {
                ID = 1,
                Name= "Java"
            });
            base.OnModelCreating(modelBuilder);//better ad for when adding other migrations
            modelBuilder.Entity<Skill>().HasData(new Skill
            {
                ID = 2,
                Name = "C"
            });
            base.OnModelCreating(modelBuilder);//better ad for when adding other migrations
            modelBuilder.Entity<Skill>().HasData(new Skill
            {
                ID = 3,
                Name = "C#"
            });
            base.OnModelCreating(modelBuilder);//better ad for when adding other migrations
            modelBuilder.Entity<Skill>().HasData(new Skill
            {
                ID = 4,
                Name = "Python"
            });
            
            //repeat for other values, then add new migration and update db
        }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Resume> Resumes { get; set; }

    }
}