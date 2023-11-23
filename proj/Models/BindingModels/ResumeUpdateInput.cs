using Microsoft.AspNetCore.Identity;
using static proj.Models.SD;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace proj.Models.BindingModels
{
    public class ResumeUpdateInput
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string email { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string? ProfilePicUrl { get; set; }
        public float grade { get; set; }
        public List<Skill> Skills { get; set; }
    }
}
