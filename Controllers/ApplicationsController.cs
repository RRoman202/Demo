using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo.Data;
using Demo.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections;

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
        [Authorize(Roles = "1, 2, 3, 4")]
        public async Task<IActionResult> Index()
        {
            var fullName = User.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
            if (User.IsInRole("4"))
            {
                var dataContext = _context.Applications.Where(a => a.ClientName == fullName).Include(a => a.Equipment).Include(a => a.Problem).Include(a => a.User).Include(a => a.status);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Applications.Include(a => a.Equipment).Include(a => a.Problem).Include(a => a.User).Include(a => a.status);
                return View(await dataContext.ToListAsync());
            }
            
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
        [Authorize(Roles = "1")]
        public IActionResult Create()
        {
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Name");
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.Where(x => x.roleId == 2), "Id", "FullName");
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application application)
        {
            

            DateTime dateTime = DateTime.Now;
            DateOnly dateOnly = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            application.DateAddition = dateOnly;
            application.WorkStatus = "Не выполнено";
            application.PeriodExecution = dateOnly.AddDays(5);
            application.DateEnd = null;
            if (ModelState.IsValid)
            {
                
                _context.Add(application);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["EquipmentTypeId"] = new SelectList(_context.EquipmentTypes, "Id", "Name", application.EquipmentTypeId);
            ViewData["ProblemTypeId"] = new SelectList(_context.ProblemTypes, "Id", "Name", application.ProblemTypeId);
            ViewData["UserId"] = new SelectList(_context.Users.Where(x => x.roleId == 2), "Id", "FullName", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", application.StatusId);
            return View(application);
        }

        // GET: Applications/Edit/5
        [Authorize(Roles = "1, 2")]
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
            ViewData["UserId"] = new SelectList(_context.Users.Where(x => x.roleId == 2), "Id", "FullName", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", application.StatusId);

            List<string> workStatuses = new List<string>
            {
                "Не выполнено",
                "В работе",
                "Выполнено"
            };
            List<SelectListItem> selectList = workStatuses.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            ViewBag.WorkStatuses = selectList;

            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1, 2")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateAdditional,NameEquipment,EquipmentTypeId,ProblemTypeId,Comment,StatusId,ClientName,Cost,DateEnd,TimeWork,UserId,WorkStatus,PeriodExecution,Number,Description")] Application application)
        {
            var user = User;
            bool roleDis = user.IsInRole("1");
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingApplication = await _context.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == application.Id);
                    if (existingApplication != null)
                    {
                        application.PeriodExecution = existingApplication.PeriodExecution;
                        application.DateAddition = existingApplication.DateAddition;
                        application.NameEquipment = existingApplication.NameEquipment;
                        application.EquipmentTypeId = existingApplication.EquipmentTypeId;
                        application.ProblemTypeId = existingApplication.ProblemTypeId;
                        application.ClientName = existingApplication.ClientName;
                        application.Cost = existingApplication.Cost;
                        application.Number = existingApplication.Number;
                        if (roleDis)
                        {
                            application.WorkStatus = existingApplication.WorkStatus;
                            application.TimeWork = existingApplication.TimeWork;
                            application.Comment = existingApplication.Comment;

                        }
                        else
                        {
                            application.UserId = existingApplication.UserId;
                            application.StatusId = existingApplication.StatusId;
                            if(existingApplication.WorkStatus != application.WorkStatus)
                            {
                                Notification notification = new Notification();
                                notification.ApplicationId = application.Id;
                                notification.Comment = $"Статус заявки [{application.Id}] изменен с [{existingApplication.WorkStatus}] на [{application.WorkStatus}]";
                                notification.UserId = Convert.ToInt64(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value);
                                notification.CreatedAt = DateTime.UtcNow;
                                _context.Notifications.Add(notification);
                            }
                            if(application.WorkStatus == "Выполнено")
                            {
                                DateTime dateTime = DateTime.Now;
                                DateOnly dateOnly = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
                                application.DateEnd = dateOnly;
                            }

                        }
                        _context.Update(application);
                        await _context.SaveChangesAsync();
                    }
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
            ViewData["UserId"] = new SelectList(_context.Users.Where(x => x.roleId == 2), "Id", "FullName", application.UserId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", application.StatusId);
            List<string> workStatuses = new List<string>
            {
                "Не выполнено",
                "В работе",
                "Выполнено"
            };
            List<SelectListItem> selectList = workStatuses.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            ViewBag.WorkStatuses = selectList;
            return View(application);
        }

        [Authorize(Roles = "3")]
        public async Task<IActionResult> EditForManager(int? id)
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

            ViewData["UserId"] = new SelectList(_context.Users.Where(x => x.roleId == 2), "Id", "FullName", application.UserId);

            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> EditForManager(int id, [Bind("Id,UserId,PeriodExecution")] Application application)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingApplication = await _context.Applications.AsNoTracking().FirstOrDefaultAsync(a => a.Id == application.Id);
                    if (existingApplication != null)
                    {
                        existingApplication.UserId = application.UserId;
                        existingApplication.PeriodExecution = application.PeriodExecution;

                        _context.Update(existingApplication);
                        await _context.SaveChangesAsync();
                    }
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

        [Authorize(Roles = "1")]
        public async Task<IActionResult> Statistic()
        {
            var applications = await _context.Applications.Include(a => a.Problem).ToListAsync();

            var applicationsCompleted = await _context.Applications.Where(a => a.StatusId == 3).Include(a => a.Problem).ToListAsync();

            var completedApplications = applications.Count(a => a.StatusId == 3);

            double averageExecutionTime = 0; 
            if (applicationsCompleted.Any())
            {
                averageExecutionTime = applicationsCompleted.Select(a => DateTime.Now.Date.Add(a.TimeWork.ToTimeSpan()))
                    .Select(ts => TimeSpan.FromHours(ts.Hour) + TimeSpan.FromMinutes(ts.Minute))
                    .DefaultIfEmpty(TimeSpan.Zero) 
                    .Average(ts => ts.TotalHours); 
            }

            var problemStatistics = applications.GroupBy(a => a.Problem?.Name)
                                                .Select(g => new
                                                {
                                                    Type = g.Key,
                                                    TotalCount = g.Count(),
                                                    CompletedCount = g.Count(a => a.StatusId == 3),
                                                    WaitingCount = g.Count(a => a.StatusId == 1),
                                                    InProgressCount = g.Count(a => a.WorkStatus == "В работе"),
                                                    AverageExecutionTime = g.Where(a => a.StatusId == 3)
                                                    .Select(a => DateTime.Now.Date.Add(a.TimeWork.ToTimeSpan()))
                                                    .Select(ts => TimeSpan.FromHours(ts.Hour) + TimeSpan.FromMinutes(ts.Minute))
                                                    .DefaultIfEmpty(TimeSpan.Zero)
                                                    .Average(ts => ts.TotalHours)
                                                });

            ViewData["CompletedApplications"] = completedApplications;
            ViewData["AverageExecutionTime"] = averageExecutionTime;
            return View(problemStatistics);
        }


    }
}
