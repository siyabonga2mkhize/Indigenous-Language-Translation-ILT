using InnoDevsITL.Data;
using InnoDevsITL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace InnoDevsITL.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class FacultiesController : Controller
    {
        private readonly InnoDbContext dbContext;
        public FacultiesController(InnoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task<IActionResult> Index()
        {
            var faculties = await dbContext.Faculties.ToListAsync();
            return View(faculties);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                dbContext.Faculties.Add(faculty);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        //EDIT:POST

        public async Task<IActionResult> Edit(int Id)
        {
            var faculty = await dbContext.Faculties.FindAsync(Id);
            if(faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, Faculty faculty)
        {
            if(Id != faculty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                dbContext.Faculties.Update(faculty);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        //DELWTW: 

        public async Task<IActionResult> Delete(int Id)
        {
           var faculty = await dbContext.Faculties.FindAsync(Id);
           if(faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var faculty = await dbContext.Faculties.FindAsync(Id);
            
            if (faculty == null)
            {
                return NotFound();
            }
            dbContext.Faculties.Remove(faculty);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
