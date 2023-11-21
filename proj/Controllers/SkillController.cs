using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model;
using proj.Data;
using proj.Models;
using proj.Services;

namespace proj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SkillController : Controller
    {
        private readonly IDatabaseRepository _db;
        public SkillView Input { get; set; }

        public SkillController(IDatabaseRepository db)
        {
            _db = db;
            Input = new SkillView();
            Input.Skills=new List<Skill>();
        }

        public List<Skill> Skills { get; set; }
        [HttpGet]
        public IActionResult Index()
        {
            Skills = _db.GetAllSkills();
            Input.Skills= Skills;
            Input.SKillInput = new Skill();
            return View(Input);
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            Skill s = _db.GetSkillById(id);
            return View(s);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Create(SkillView skillView)
        {
            // Check if ModelState is valid
                // Add the skill from the input model
                _db.AddSkill(skillView.SKillInput);

                // Redirect to the Index action
                return RedirectToAction("Index");
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Update(Skill s)
        {
            // Check if ModelState is valid
            // Add the skill from the input model
            _db.UpdateSkill(s);

            // Redirect to the Index action
            return RedirectToAction("Index");
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Delete(int id)
        {
            // Check if ModelState is valid
            // Add the skill from the input model

            _db.DeleteSkill(_db.GetSkillById(id));

            // Redirect to the Index action
            return RedirectToAction("Index");
        }
    }
}
