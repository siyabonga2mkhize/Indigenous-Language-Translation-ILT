    using InnoDevsITL.Data;
    using InnoDevsITL.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Authorization;

    namespace InnoDevsITL.Controllers
    {
        //[Authorize(Roles = "Admin")]
        public class CampusesController : Controller
        {
            private readonly InnoDbContext dbContext;
            public CampusesController(InnoDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<IActionResult> Index()
            {
                var campuses = await dbContext.Campuses.ToListAsync();
                return View(campuses);
            }
            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Create(Campus campus)
            {
                if (ModelState.IsValid)
                {
                    dbContext.Campuses.Add(campus);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(campus);
            }

            //EDIT:POST

            public async Task<IActionResult> Edit(int Id)
            {
                var campus = await dbContext.Campuses.FindAsync(Id);
                if (campus == null)
                {
                     return NotFound();
                }
                return View(campus);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(int Id, Campus campus)
            {
                if (Id != campus.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    dbContext.Campuses.Update(campus);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(campus);
            }

            //DELWTE
            //: 

            public async Task<IActionResult> Delete(int Id)
            {
                var campus = await dbContext.Campuses.FindAsync(Id);
                if (campus == null)
                {
                    return NotFound();
                }
                return View(campus);
            }


            [HttpPost, ActionName("Delete")]
            public async Task<IActionResult> DeleteConfirmed(int Id)
            {
                var campus = await dbContext.Campuses.FindAsync(Id);
                
                if (campus == null)
                {
                    return NotFound();
                }
                dbContext.Campuses.Remove(campus);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
    }