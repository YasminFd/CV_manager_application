using proj.Data;
using proj.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace proj.Services
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly ApplicationDbContext _db;

        public DatabaseRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddResume(Resume resume)
        {
            _db.Resumes.Add(resume);
            await _db.SaveChangesAsync();
        }

        public async Task AddSkill(Skill skill)
        {
            _db.Skills.Add(skill);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteResume(int? id)
        {
            Resume r = _db.Resumes.First(s => s.Id == id);
            if (r != null)
            {
                _db.Resumes.Remove(r);
                await _db.SaveChangesAsync();
            }
        }

        public List<Resume> GetAllResumes()
        {
            List<Resume> list = _db.Resumes.ToList();
            return list;
        }

        public List<Skill> GetAllSkills()
        {
            List<Skill> skills = _db.Skills.ToList();
            return skills;
        }

        public Resume GetResumeById(int? id)
        {
            Resume r = _db.Resumes.FirstOrDefault(s => s.Id == id);
            return r;
        }

        public Resume GetResumeWithSkillsById(int? id)
        {
            return _db.Resumes
                .Include(r => r.Skills)  // Use the Include method from Microsoft.EntityFrameworkCore
                .FirstOrDefault(m => m.Id == id);
        }

        public Skill GetSkillById(int? id)
        {
            Skill r = _db.Skills.FirstOrDefault(s => s.ID == id);
            return r;
        }

        public async Task UpdateResume(Resume resume)
        {
            _db.Resumes.Update(resume);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateSkill(Skill skill)
        {
            _db.Skills.Update(skill);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteSkill(Skill skill)
        {
            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
        }
    }
}
