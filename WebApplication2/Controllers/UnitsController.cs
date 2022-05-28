#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class UnitsController : Controller
    {
        private readonly DBaseContext _context;

        public UnitsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: Units
        public async Task<IActionResult> Index(int pg = 1)
        {
            var units = await _context.Units.FromSqlRaw("dbo.indexUnit").ToListAsync();
            //IQueryable<Unit> units = _context.Units;
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = units.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<Unit> data = units.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Units/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var unit = await _context.Units.FromSqlRaw("dbo.selectByIdUnit @It", Id).ToListAsync();

            //var unit = await _context.Units
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (unit.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(unit.FirstOrDefault());
        }

        // GET: Units/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Units/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Unit unit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(unit);
                    //await _context.SaveChangesAsync();
                    SqlParameter Title = new SqlParameter("@Title", unit.Title);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createUnit @Title", Title);

                    return RedirectToAction(nameof(Index));
                }
                return View(unit);
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Units/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var unit = await _context.Units.FromSqlRaw("dbo.selectByIdUnit @Id", Id).ToListAsync();

            //var unit = await _context.Units.FindAsync(id);
            if (unit.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(unit.FirstOrDefault());
        }

        // POST: Units/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Unit unit)
        {
            if (id != unit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(unit);
                    //await _context.SaveChangesAsync();
                    SqlParameter Id = new SqlParameter("@Id", unit.Id);
                    SqlParameter Title = new SqlParameter("@Title", unit.Title);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editUnit @Id, @Title", Id, Title);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitExists(unit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch(SqlException ex)
                {
                    return NotFound(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unit);
        }

        // GET: Units/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var unit = await _context.Units.FromSqlRaw("dbo.selectByIdUnit @Id", Id).ToListAsync();
            //var unit = await _context.Units
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (unit.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(unit.FirstOrDefault());
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var unit = await _context.Units.FindAsync(id);
            //_context.Units.Remove(unit);
            //await _context.SaveChangesAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteUnit @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.Id == id);
        }
    }
}
