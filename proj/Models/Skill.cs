using System.ComponentModel.DataAnnotations;

namespace proj.Models
{
    public class Skill
    {
        [Key]
        public int ID {  get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Resume> Resumes { get; set; }
    }
}
