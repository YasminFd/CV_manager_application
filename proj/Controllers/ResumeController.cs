using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using proj.Models;
using proj.Models.BindingModels;
using proj.Services;
using SQLitePCL;
using System.Security.Claims;
using static proj.Models.SD;

namespace proj.Controllers
{
    [Authorize]
    public class ResumeController : Controller
    {
        private readonly IDatabaseRepository _db;
        private readonly UserManager<IdentityUser> _user;
        private readonly IMapper _mapper;
        public ResumeUpdateView r { get; set; } 
        public Resume resume { get; set; }
        public ResumeController(IDatabaseRepository db,IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _user = userManager;
            Input = new ResumeCreateView();
        }
        //[BindProperty]
        public ResumeCreateView Input { get; set; }
        [HttpGet]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> Create()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (await _db.UserHasResume(userId))
            {
                TempData["Error"] = "User already has a resume";
                RedirectToAction("List");
            }
            
            Input.skills= _db.GetAllSkills();
            return View(Input);
        }
        [HttpGet]
        [Authorize(Roles = "Visitor")]
        public IActionResult List()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Resume> r = _db.GetResumeForUser(userId);
            return View(r);
        }
        [HttpGet]
        [Authorize(Roles = "Visitor")]
        public IActionResult Index(int id)
        {
            Resume r = _db.GetResumeWithSkillsById(id);
            return View(r);
        }
        [HttpGet]
        [Authorize(Roles = "Visitor")]
        public IActionResult Update(int id)
        {
            r = new ResumeUpdateView();
            resume = _db.GetResumeWithSkillsById(id);
            r.Input = _mapper.Map<ResumeUpdateInput>(resume);
            if (r.Input == null)
            {
                // Handle the case where the resume with the given id is not found.
                return NotFound();
            }

            r.skills = _db.GetAllSkills();

            // Initialize the SkillsCheckboxes list with all false values.
            r.SkillsCheckboxes = new List<bool>(r.skills.Count);
            for (int i = 0; i < r.skills.Count; i++)
            {
                r.SkillsCheckboxes.Add(false);
            }

            // Check and set true for the skills that are associated with the resume.
            if (r.Input.Skills != null)
            {
                foreach (var skill in r.Input.Skills)
                {
                    int index = r.skills.FindIndex(s => s.ID == skill.ID);

                    // Check if the skill is found in the list.
                    if (index != -1)
                    {
                        r.SkillsCheckboxes[index] = true;
                    }
                }
            }

            return View(r);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _db.DeleteResume(id);
            TempData["Message"] = "Resume deleted successfully!";
            return RedirectToAction("List");
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> CreateAsync(ResumeCreateView v)
        {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (await _db.UserHasResume(userId)) RedirectToAction("List");
                v.skills = _db.GetAllSkills();

            if (v.Input.ProfileImage == null || !ImageUploadService.CheckExtensionValidity(v.Input.ProfileImage)) 
             {
                ModelState.AddModelError("Input.ProfileImage", "Please choose a valid image form");
             }
            if (DateService.checkIfPastDate(v.Input.BirthDate))
            {
                ModelState.AddModelError("Input.BirthDate", "Choose a date that hasn't passed yet");
            }
            if (DateService.checkMinimumAge(v.Input.BirthDate))
            {
                ModelState.AddModelError("Input.BirthDate", "You should be at least 18 years old");
            }
            if ((v.Input.Number1 + v.Input.Number2)!=v.Input.Number3)
            {
                ModelState.AddModelError("Input.Number3", "Choose correct numbers");
            }
            if (!ModelState.IsValid)
            {
                foreach (var (key, value) in ModelState)
                {
                    var errors = value.Errors;

                    foreach (var error in errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        Console.WriteLine($"Error in {key}: {errorMessage}");
                    }
                }
                return View(v);
            }
            Resume r = _mapper.Map<Resume>(v.Input);
            if (v.Input.ProfileImage != null)
            {
                r.ProfilePicUrl = ImageUploadService.UploadFile(v.Input.ProfileImage);
            }
            
            var increment = 0;
            if (v.Input.Gender.Equals("Female"))
            {
                increment = 10;
            }
            else
            {
                increment = 5;
            }
            r.Skills=new List<Skill>();
            for (int i = 0; i < v.skills.Count; i++)
            {
                if (v.Input.SkillsCheckboxes[i] == true)
                {
                    //need to save the corresponding skill[i] as a skill
                    r.Skills.Add(v.skills[i]);
                    r.grade += increment;
                }
            }
            r.user = await _user.GetUserAsync(User);
            await _db.AddResume(r);
            TempData["Message"] ="Resume created successfully!";
            return RedirectToAction("Index", new { id = r.Id });

        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> UpdateAsync(ResumeUpdateView v)
        {
            var user = await _user.GetUserAsync(HttpContext.User);
            v.skills = _db.GetAllSkills();
            if (v.Pic != null && !ImageUploadService.CheckExtensionValidity(v.Pic))
            {
                ModelState.AddModelError("Input.ProfileImage", "Please choose a valid image form");
            }
            if (DateService.checkIfPastDate(v.Input.BirthDate))
            {
                ModelState.AddModelError("Input.BirthDate", "Choose a date that hasn't passed yet");
            }
            if (DateService.checkMinimumAge(v.Input.BirthDate))
            {
                ModelState.AddModelError("Input.BirthDate", "You should be at least 18 years old");
            }
            if (!ModelState.IsValid)
            {
                foreach (var (key, value) in ModelState)
                {
                    var errors = value.Errors;

                    foreach (var error in errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        Console.WriteLine($"Error in {key}: {errorMessage}");
                    }
                }
                // Check validation errors specifically for the Input.user field.
                var userErrors = ModelState["Input.user"]?.Errors;
                if (userErrors != null && userErrors.Any())
                {
                    foreach (var error in userErrors)
                    {
                        Console.WriteLine($"Error in Input.user: {error.ErrorMessage}");
                    }
                }
                return View(v);
            }
            if (v.Pic != null)
            {
                v.Input.ProfilePicUrl = ImageUploadService.UploadFile(v.Pic);
            }
            v.Input.grade = 0;
            var increment = 0;
            if (v.Input.Gender.Equals("Female"))
            {
                increment = 10;
            }
            else
            {
                increment = 5;
            }
            v.Input.grade += increment;
            v.Input.Skills = new List<Skill>();
            for (int i = 0; i < v.skills.Count; i++)
            {
                if (v.SkillsCheckboxes[i] == true)
                {
                    //need to save the corresponding skill[i] as a skill
                    v.Input.Skills.Add(v.skills[i]);
                    v.Input.grade += increment;
                }
            }
            Resume r = _mapper.Map<Resume>(v.Input);

            foreach (var skill in v.Input.Skills)
            {
                Console.WriteLine($"Resume ID: {r.Id}, Skill ID: {skill.ID}");
            }
            r.user = await _user.GetUserAsync(User);
            await _db.UpdateResume(r);
            TempData["Message"] = "Resume updated successfully!";
            return RedirectToAction("Index", new { id = r.Id });

        }
    }


    public class ResumeCreateView
    {

        public ResumeInput Input { get; set; } = new ResumeInput();
        public Nationalities Nationalities { get; set; }
        public Gender Gender { get; set; }
        [BindNever]
        public List<Skill>? skills { get; set; }
    }
    public class ResumeUpdateView
    {
        public ResumeUpdateInput Input { get; set; }
        public IFormFile? Pic { get; set; }
        public List<bool>? SkillsCheckboxes { get; set; }
        public Nationalities Nationalities { get; set; }
        public Gender Gender { get; set; }
        [BindNever]
        public List<Skill>? skills { get; set; }
    }
}
