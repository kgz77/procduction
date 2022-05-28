#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly DBaseContext _context;
        public BudgetsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            var budget = await _context.Budgets.FromSqlRaw("dbo.indexBudget").ToListAsync();
            return View(budget);
        }

        // GET: Budgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var budget = await _context.Budgets.FromSqlRaw("dbo.selectByIdBudget @Id", Id).ToListAsync();

            if (budget.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(budget.FirstOrDefault());
        }

        // GET: Budgets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Add(budget);
                    //await _context.SaveChangesAsync();
                    SqlParameter SumOfBudget = new SqlParameter("@SumOfBudget", budget.SumOfBudget);
                    SqlParameter PercentageOfPremium = new SqlParameter("@PercentageOfPremium", budget.PercentageOfPremium);
                    SqlParameter Bonus = new SqlParameter("@Bonus", budget.Bonus);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createBudget @SumOfBudget, @PercentageOfPremium, @Bonus", SumOfBudget, PercentageOfPremium, Bonus);
             
                    return RedirectToAction(nameof(Index));
                }
                catch(SqlException ex)
                {
                    //ViewBag.Message = ex.Message.ToString();
                    return NotFound(ex.Message);
                }

            }
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var budget = await _context.Budgets.FromSqlRaw("dbo.selectByIdBudget @Id", Id).ToListAsync();
            //var budget = await _context.Budgets.FindAsync(id);
            if (budget.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(budget.FirstOrDefault());
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", budget.Id);
                    SqlParameter SumOfBudget = new SqlParameter("@SumOfBudget", budget.SumOfBudget);
                    SqlParameter PercentageOfPremium = new SqlParameter("@PercentageOfPremium", budget.PercentageOfPremium);
                    SqlParameter Bonus = new SqlParameter("@Bonus", budget.Bonus);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editBudget @Id, @SumOfBudget, @PercentageOfPremium, @Bonus", Id, SumOfBudget, PercentageOfPremium, Bonus);

                    //_context.Update(budget);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.Id))
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
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SqlParameter Id = new SqlParameter("@Id", id);
            var budget = await _context.Budgets.FromSqlRaw("dbo.selectByIdBudget @Id", Id).ToListAsync();
            //var budget = await _context.Budgets
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (budget.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(budget.FirstOrDefault());

        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                SqlParameter Id = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteBudget @Id", Id);

                //var budget = await _context.Budgets.FindAsync(id);
                //_context.Budgets.Remove(budget);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.Id == id);
        }
    }
}
