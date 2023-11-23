using proj.Models;

namespace proj.Services
{
    public interface IDatabaseRepository
    {
        public List<Skill> GetAllSkills();
        public List<Resume> GetAllResumes();
        public Task AddResume(Resume resume);
        public Task AddSkill(Skill skill);
        public Task UpdateResume(Resume resume);
        public Resume GetResumeWithSkillsById(int? id);
        public Resume GetResumeById(int? id);
        public Skill GetSkillById(int? id);
        public Task DeleteResume(int? id);
        public Task UpdateSkill(Skill skill);
        public Task DeleteSkill(Skill skill);
        public Skill GetSkillWithResume(int? id);
        public List<Resume> GetAllResumesForUser(string id);
    }

}

