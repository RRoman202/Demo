using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo.Data;
using Demo.Data.Models;

namespace Demo.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly DataContext _context;

        public ApplicationsController(DataContext context)
        {
            _context = context;
        }

        // GET: Applications
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Applications.Include(a => a.Equipment).Include(a => a.Problem).Include(a => a.User).Include(a => a.status);
            return View(await dataContext.ToListAsync());
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Equipment)
                .Include(a => a.Problem)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        public IActionResult Create()
        {
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Name");
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application)
        {
            

            DateTime dateTime = DateTime.Now;
            DateOnly dateOnly = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            application.DateAddition = dateOnly;
            if (ModelState.IsValid)
            {
                
                _context.Add(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Name", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Name", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", application.StatusId);
            return View(application);
        }

        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Name", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Name", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", application.StatusId);
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateAddition,NameEquipment,EquipmentTypeId,ProblemTypeId,Comment,StatusId,ClientName,Cost,DateEnd,TimeWork,UserId,WorkStatus,PeriodExecution,Number,Description")] Application application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Name", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Name", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", application.StatusId);
            return View(application);
        }

        // GET: Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Equipment)
                .Include(a => a.Problem)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application != null)
            {
                _context.Applications.Remove(application);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }
}
