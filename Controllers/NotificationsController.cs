using Demo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Controllers
{
    public class NotificationsController : Controller
    {

        private readonly DataContext _context;

        public NotificationsController(DataContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1, 4")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("4"))
            {
                var fullName = User.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                var dataContext = _context.Notifications.Where(a => a.Application.ClientName == fullName).Include(a => a.User).Include(a => a.Application);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Notifications.Include(a => a.User).Include(a => a.Application);
                return View(await dataContext.ToListAsync());
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1, 4")]
        public async Task<IActionResult> Delete(int id)
        {
            var noti = await _context.Notifications.FindAsync(id);
            if (noti != null)
            {
                _context.Notifications.Remove(noti);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Notifications");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1, 4")]
        public async Task<IActionResult> DeleteAll()
        {
            if (User.IsInRole("4"))
            {
                var fullName = User.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                var notifications = await _context.Notifications.Where(a => a.Application.ClientName == fullName).ToListAsync();
                _context.Notifications.RemoveRange(notifications);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Notifications");
            }
            else
            {
                var notifications = await _context.Notifications.ToListAsync();
                _context.Notifications.RemoveRange(notifications);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Notifications");
            }
            
        }
    }
}
