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
    public class RawMaterialsController : Controller
    {
        private readonly DBaseContext _context;

        public RawMaterialsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: RawMaterials
        public async Task<IActionResult> Index(int pg = 1)
        {
            var dataRawMaterial = await _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToListAsync();
            var dataUnit = await _context.Units.FromSqlRaw("dbo.indexUnit").ToListAsync();
          
            foreach (var r in dataRawMaterial)
            {
                foreach (var u in dataUnit)
                {
                    if (r.Unit == u.Id)
                    {
                        r.UnitNavigation.Title = u.Title;
                    }
                }
            }

            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = dataRawMaterial.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<RawMaterial> data = dataRawMaterial.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);

        }

        // GET: RawMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var dataUnit = await _context.Units.FromSqlRaw("dbo.indexUnit").ToListAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            var dataRawMaterial = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @Id", Id).ToListAsync();

            foreach (var r in dataRawMaterial)
            {
                foreach (var u in dataUnit)
                {
                    if (r.Unit == u.Id)
                    {
                        r.UnitNavigation.Title = u.Title;
                    }
                }
            }

            //var rawMaterial = await _context.RawMaterials
            //    .Include(r => r.UnitNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (dataRawMaterial.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(dataRawMaterial.FirstOrDefault());
        }

        // GET: RawMaterials/Create
        public IActionResult Create()
        {
            var dataUnit = _context.Units.FromSqlRaw("dbo.indexUnit").ToList();

            ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title");

            return View();
        }

        // POST: RawMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RawMaterial rawMaterial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(rawMaterial);
                    //await _context.SaveChangesAsync();
                    SqlParameter Title = new SqlParameter("@Title", rawMaterial.Title);
                    SqlParameter Unit = new SqlParameter("@Unit", rawMaterial.Unit);
                    SqlParameter Summa = new SqlParameter("@Summa", rawMaterial.Summa);
                    SqlParameter Amount = new SqlParameter("@Amount", rawMaterial.Amount);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createRawMaterial @Title, @Unit, @Summa, @Amount", Title, Unit, Summa, Amount);

                    return RedirectToAction(nameof(Index));
                }
                var dataUnit = _context.Units.FromSqlRaw("dbo.indexUnit").ToList();
                ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title", rawMaterial.Unit);
                return View(rawMaterial);
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: RawMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            SqlParameter Id = new SqlParameter("@Id", id);
            var dataRawMaterial = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @Id", Id).ToListAsync(); 
            var dataUnit = _context.Units.FromSqlRaw("dbo.indexUnit").ToList();

            if (id == null)
            {
                return NotFound();
            }

            if (dataRawMaterial.FirstOrDefault() == null)
            {
                return NotFound();
            }
            ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title", dataRawMaterial.FirstOrDefault().Unit);
            return View(dataRawMaterial.FirstOrDefault());
        }

        // POST: RawMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RawMaterial rawMaterial)
        {
            if (id != rawMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", rawMaterial.Id);
                    SqlParameter Title = new SqlParameter("@Title", rawMaterial.Title);
                    SqlParameter Unit = new SqlParameter("@Unit", rawMaterial.Unit);
                    SqlParameter Summa = new SqlParameter("@Summa", rawMaterial.Summa);
                    SqlParameter Amount = new SqlParameter("@Amount", rawMaterial.Amount);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editRawMaterial @Id, @Title, @Unit, @Summa, @Amount", Id, Title, Unit, Summa, Amount);
                    //_context.Update(rawMaterial);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RawMaterialExists(rawMaterial.Id))
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
            var dataUnit = _context.Units.FromSqlRaw("dbo.indexUnit").ToList();
            ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title", rawMaterial.Unit);
            return View(rawMaterial);
        }

        // GET: RawMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var dataUnit = await _context.Units.FromSqlRaw("dbo.indexUnit").ToListAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            var dataRawMaterial = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @Id", Id).ToListAsync();

            foreach (var r in dataRawMaterial)
            {
                foreach (var u in dataUnit)
                {
                    if (r.Unit == u.Id)
                    {
                        r.UnitNavigation.Title = u.Title;
                    }
                }
            }
            //var rawMaterial = await _context.RawMaterials
            //    .Include(r => r.UnitNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (dataRawMaterial.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(dataRawMaterial.FirstOrDefault());
        }

        // POST: RawMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var rawMaterial = await _context.RawMaterials.FindAsync(id);
            //_context.RawMaterials.Remove(rawMaterial);
            //await _context.SaveChangesAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteRawMaterial @Id", Id);

            return RedirectToAction(nameof(Index));
        }

        private bool RawMaterialExists(int id)
        {
            return _context.RawMaterials.Any(e => e.Id == id);
        }
    }
}
