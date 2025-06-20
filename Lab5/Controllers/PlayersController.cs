using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab3.Data;
using Lab3.Models;
using Microsoft.AspNetCore.Authorization;

namespace Lab3.Controllers
{
    [Authorize]
    public class PlayersController : Controller
    {
        private readonly RosterContext _context;

        public PlayersController(RosterContext context)
        {
            _context = context;
        }
        private int CalculateAge(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age)) age--;
            return age;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            return View(await _context.Players.ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.PlayerId == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("PlayerId,Jersey,FName,SName,Position,Birthday,Weight,Height,BirthCity,BirthState")] Player player)
        {
            if (ModelState.IsValid)
            {
                var age = CalculateAge(player.Birthday);

                if (age < 20 && player.Height < 180 && player.Weight >= 85)
                {
                    ModelState.AddModelError("", "Игрок моложе 20 лет с ростом < 180 должен весить меньше 85 кг.");
                    return View(player);
                }
                if (player.Jersey < 10 && player.Position == "C" && player.Weight >= 90)
                {
                    ModelState.AddModelError("", "Игрок с номером на майке меньше 10, играющий на позиции \"C\"(центровой), не может весить больше 90 кг.");
                    return View(player);
                }
                if (player.FName.StartsWith("A") && player.Height > 190 && player.Position == "RW")
                {
                    ModelState.AddModelError("", "Игроки с именем, начинающимся на \"A\", и ростом выше 190 см, не могут играть на позиции \"RW\".");
                    return View(player);
                }

                if (age > 35)
                {
                    ModelState.AddModelError("", "Игроки старше 35 лет не допускаются.");
                    return View(player);
                }
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(player);
        }

        // GET: Players/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("PlayerId,Jersey,FName,SName,Position,Birthday,Weight,Height,BirthCity,BirthState")] Player player)
        {
            if (id != player.PlayerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var age = CalculateAge(player.Birthday);

                    if (age < 20 && player.Height < 180 && player.Weight >= 85)
                    {
                        ModelState.AddModelError("", "Игрок моложе 20 лет с ростом < 180 должен весить меньше 85 кг.");
                        return View(player);
                    }
                    if (player.Jersey < 10 && player.Position == "C" && player.Weight >= 90)
                    {
                        ModelState.AddModelError("", "Игрок с номером на майке меньше 10, играющий на позиции \"C\"(центровой), не может весить больше 90 кг.");
                        return View(player);
                    }
                    if (player.FName.StartsWith("A") && player.Height > 190 && player.Position == "RW")
                    {
                        ModelState.AddModelError("", "Игроки с именем, начинающимся на \"A\", и ростом выше 190 см, не могут играть на позиции \"RW\".");
                        return View(player);
                    }
                    if (age > 35)
                    {
                        ModelState.AddModelError("", "Игроки старше 35 лет не допускаются.");
                        return View(player);
                    }

                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.PlayerId))
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
            return View(player);
        }

        // GET: Players/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.PlayerId == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(string id)
        {
            return _context.Players.Any(e => e.PlayerId == id);
        }
    }
}
