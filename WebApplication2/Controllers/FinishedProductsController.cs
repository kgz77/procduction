#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using PagedList.Mvc;
using PagedList;
using Microsoft.Data.SqlClient;

namespace WebApplication2.Controllers
{
    public class FinishedProductsController : Controller
    {
        private readonly DBaseContext _context;

        public FinishedProductsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: FinishedProducts
        public async Task<IActionResult> Index(int pg = 1)
        {
            var dataFinishedProduct = await _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToListAsync();
            var dataUnit = await _context.Units.FromSqlRaw("dbo.indexUnit").ToListAsync();

            foreach (var f in dataFinishedProduct)
            {
                foreach (var u in dataUnit)
                {
                    if (f.Unit == u.Id)
                    {
                        f.UnitNavigation.Title = u.Title;
                    }
                }
            }
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = dataFinishedProduct.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<FinishedProduct> data = dataFinishedProduct.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            return View(data);
        }

        // GET: FinishedProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var finishedProduct = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @Id", Id).ToListAsync();

            SqlParameter IdUnit = new SqlParameter("@IdPost", finishedProduct[0].Unit);
            var unit = await _context.Units.FromSqlRaw("dbo.selectByIdUnit @IdPost", IdUnit).ToListAsync();

            if (finishedProduct.FirstOrDefault().Unit == unit.FirstOrDefault().Id)
                finishedProduct.FirstOrDefault().UnitNavigation.Title = unit.FirstOrDefault().Title;
            //var finishedProduct = await _context.FinishedProducts
            //    .Include(f => f.UnitNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (finishedProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(finishedProduct.FirstOrDefault());
        }

        // GET: FinishedProducts/Create
        public IActionResult Create()
        {
            var dataUnit = _context.Units.FromSqlRaw("dbo.indexUnit").ToList();
            ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title");
            return View();
        }

        // POST: FinishedProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinishedProduct finishedProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Title = new SqlParameter("@Title", finishedProduct.Title);
                    SqlParameter Unit = new SqlParameter("@Unit", finishedProduct.Unit);
                    SqlParameter Summa = new SqlParameter("@Summa", finishedProduct.Summa);
                    SqlParameter Amount = new SqlParameter("@Amount", finishedProduct.Amount);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createFinishedProduct @Title, @Unit, @Summa, @Amount", Title, Unit, Summa, Amount);

                    //_context.Add(finishedProduct);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                var dataUnit = _context.Units.FromSqlRaw("dbo.indexUnit").ToList();
                ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title", finishedProduct.Unit);
                return View(finishedProduct);
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: FinishedProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var finishedProduct = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @Id", Id).ToListAsync();

            //var finishedProduct = await _context.FinishedProducts.FindAsync(id);
            if (finishedProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }
            var dataUnit = await _context.Units.FromSqlRaw("dbo.indexUnit").ToListAsync();
            ViewData["Unit"] = new SelectList(dataUnit, "Id", "Title", finishedProduct.FirstOrDefault().Unit);
            return View(finishedProduct.FirstOrDefault());
        }

        // POST: FinishedProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FinishedProduct finishedProduct)
        {
            if (id != finishedProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", finishedProduct.Id);
                    SqlParameter Title = new SqlParameter("@Title", finishedProduct.Title);
                    SqlParameter Unit = new SqlParameter("@Unit", finishedProduct.Unit);
                    SqlParameter Summa = new SqlParameter("@Summa", finishedProduct.Summa);
                    SqlParameter Amount = new SqlParameter("@Amount", finishedProduct.Amount);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editFinishedProduct @Id, @Title, @Unit, @Summa, @Amount",Id, Title, Unit, Summa, Amount);

                    //_context.Update(finishedProduct);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinishedProductExists(finishedProduct.Id))
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
            ViewData["Unit"] = new SelectList(_context.Units, "Id", "Title", finishedProduct.Unit);
            return View(finishedProduct);
        }

        // GET: FinishedProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var finishedProduct = await _context.FinishedProducts
            //    .Include(f => f.UnitNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var finishedProduct = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @Id", Id).ToListAsync();
            SqlParameter IdUnit = new SqlParameter("@IdPost", finishedProduct.FirstOrDefault().Unit);
            var  unit = await _context.Units.FromSqlRaw("dbo.selectByIdUnit @IdPost", IdUnit).ToListAsync();

            if (finishedProduct.FirstOrDefault().Unit == unit.FirstOrDefault().Id)
                finishedProduct.FirstOrDefault().UnitNavigation.Title = unit.FirstOrDefault().Title;
            if (finishedProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(finishedProduct.FirstOrDefault());
        }

        // POST: FinishedProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var finishedProduct = await _context.FinishedProducts.FindAsync(id);
            //_context.FinishedProducts.Remove(finishedProduct);
            //await _context.SaveChangesAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteFinishedProduct @Id", Id);

            return RedirectToAction(nameof(Index));
        }

        private bool FinishedProductExists(int id)
        {
            return _context.FinishedProducts.Any(e => e.Id == id);
        }
    }
}
