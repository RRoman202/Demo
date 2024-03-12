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
    public class Applications1Controller : Controller
    {
        private readonly DataContext _context;

        public Applications1Controller(DataContext context)
        {
            _context = context;
        }

        // GET: Applications1
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Applications.Include(a => a.Equipment).Include(a => a.Problem).Include(a => a.User).Include(a => a.status);
            return View(await dataContext.ToListAsync());
        }

        // GET: Applications1/Details/5
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
                .Include(a => a.status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications1/Create
        public IActionResult Create()
        {
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Id");
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id");
            return View();
        }

        // POST: Applications1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateAddition,NameEquipment,EquipmentTypeId,ProblemTypeId,Comment,StatusId,ClientName,Cost,DateEnd,TimeWork,UserId,WorkStatus,PeriodExecution,Number,Description")] Application application)
        {
            if (ModelState.IsValid)
            {
                _context.Add(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Id", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Id", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", application.StatusId);
            return View(application);
        }

        // GET: Applications1/Edit/5
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
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Id", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Id", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", application.StatusId);
            return View(application);
        }

        // POST: Applications1/Edit/5
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
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Id", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Id", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Id", application.StatusId);
            return View(application);
        }

        // GET: Applications1/Delete/5
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
                .Include(a => a.status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications1/Delete/5
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
