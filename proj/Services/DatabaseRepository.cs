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
            try
            {
                Resume r = GetResumeWithSkillsById(id);
                if (r != null)
                {
                    _db.Resumes.Remove(r);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine(ex.Message);
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
                .Include(r => r.Skills)
                .Include(r => r.user)// Use the Include method from Microsoft.EntityFrameworkCore
                .FirstOrDefault(m => m.Id == id);
        }

        public Skill GetSkillById(int? id)
        {
            Skill r = _db.Skills.FirstOrDefault(s => s.ID == id);
            return r;
        }

        public async Task UpdateResume(Resume resume)
        {
            try
            {
                // Check if the resume exists
                var existingResume = await _db.Resumes
                    .Include(r => r.Skills) // Include the Skills navigation property
                    .FirstOrDefaultAsync(r => r.Id == resume.Id);

                if (existingResume == null)
                {
                    // Handle case where the resume does not exist
                    // (you may want to log this or return an appropriate response)
                    return;
                }

                // Update resume properties
                existingResume.BirthDate = resume.BirthDate;
                existingResume.FirstName = resume.FirstName;
                existingResume.Gender = resume.Gender;
                existingResume.LastName = resume.LastName;
                existingResume.Nationality = resume.Nationality;
                existingResume.PhoneNumber = resume.PhoneNumber;
                existingResume.ProfilePicUrl = resume.ProfilePicUrl;
                existingResume.email = resume.email;
                existingResume.grade = resume.grade;

                // Remove skills that are no longer associated
                foreach (var existingSkill in existingResume.Skills.ToList())
                {
                    if (!resume.Skills.Any(updatedSkill => updatedSkill.ID == existingSkill.ID))
                    {
                        // Skill is no longer associated, so remove it
                        existingResume.Skills.Remove(existingSkill);
                    }
                }

                // Add new skills or update existing ones
                foreach (var updatedSkill in resume.Skills)
                {
                    var existingSkill = existingResume.Skills.FirstOrDefault(s => s.ID == updatedSkill.ID);

                    if (existingSkill == null)
                    {
                        // Skill is not associated with the resume, so add it
                        existingResume.Skills.Add(updatedSkill);
                    }
                }

                // Save changes
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, return an error response, etc.)
                Console.WriteLine($"An error occurred while updating the resume: {ex.Message}");
                throw; // Rethrow the exception to propagate it further if needed
            }
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

        public List<Resume> GetResumeForUser(string id)
        {
            return _db.Resumes.Where(i=> i.user.Id.Equals(id)).ToList();
        }

        public Skill GetSkillWithResume(int? id)
        {
            return _db.Skills
               .Include(r => r.Resumes)  // Use the Include method from Microsoft.EntityFrameworkCore
               .FirstOrDefault(m => m.ID == id);
        }

        public async Task<bool> UserHasResume(string userId)
        {
            // Check if a resume exists for the specified userId
            return await _db.Resumes.AnyAsync(r => r.user.Id == userId);
        }

        public List<Resume> GetAllResumesWithUser()
        {
            return _db.Resumes
        .Include(r => r.user)
        .ToList();
        }
    }
}
