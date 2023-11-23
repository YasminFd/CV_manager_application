using Humanizer;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static proj.Models.SD;

namespace proj.Models
{
    public class Resume
    {
        public int Id { get; set; }
        public IdentityUser user { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string email { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string? ProfilePicUrl { get; set; }
        public float grade { get; set; }
        public ICollection<Skill> Skills { get; set; }

    }
}
