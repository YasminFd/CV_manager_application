using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using proj.Models;
using proj.Models.BindingModels;
using proj.Services;
using System.Security.Claims;
using static proj.Models.SD;

namespace proj.Controllers
{
    [Authorize(Roles = "Visitor")]
    public class ResumeController : Controller
    {
        private readonly IDatabaseRepository _db;
        private readonly UserManager<IdentityUser> _user;
        private readonly IMapper _mapper;
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
        public IActionResult Create()
        {
            Input.skills= _db.GetAllSkills();
            return View(Input);
        }
        [HttpGet]
        public IActionResult Index(int id)
        {
            Resume r = _db.GetResumeWithSkillsById(id);
            return View(r);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _db.DeleteResume(id);
            return RedirectToAction("");
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateAsync(ResumeCreateView v)
        {
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
                return View();
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
            r.email= User.FindFirstValue(ClaimTypes.Email);
            r.user = await _user.GetUserAsync(User);
            await _db.AddResume(r);
            return RedirectToAction("Index",r.Id);
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
}
