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
    public class SalariesController : Controller
    {
        private readonly DBaseContext _context;

        public SalariesController(DBaseContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index(string getYear, string getMonth)
        {
            var salary = await _context.Salaries.FromSqlRaw("dbo.indexSalary").ToListAsync();

            var emp = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();
            var month = await _context.Months.FromSqlRaw("dbo.indexMonth").ToListAsync();

            ViewData["month"] = new SelectList(month, "Id", "Month1");

            salary.ForEach(i => { 
                emp.ForEach(j => { if (i.Employee == j.Id)  i.EmployeeNavigation.Names = j.Names; }); 
                month.ForEach(k => { if (i.Month == k.Id)  i.MonthNavigation.Month1 = k.Month1; }); });

            if (!String.IsNullOrEmpty(getYear) && !String.IsNullOrEmpty(getMonth))
            {

                var s = salary.Where(i => i.Date == Convert.ToInt32(getYear) && i.Month == Convert.ToInt32(getMonth)).ToList();
                s.Reverse();
                return View(s);
            }
            else
            {
                salary.Reverse();
                return View(salary);
            }


        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Salaries == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.EmployeeNavigation)
                .Include(s => s.MonthNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            var month = _context.Months.FromSqlRaw("dbo.indexMonth").ToList();
            ViewData["Month"] = new SelectList(month, "Id", "Month1");
            return View();
        }

        //private int countInPurchase(Em)
        static List<Salary> salary = new List<Salary>();
        //public void create(Salary salary)
        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(int Date, int Month)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            SqlParameter y = new SqlParameter("@y", Date);
        //            SqlParameter m = new SqlParameter("@m", Month);

        //            var outParam = new SqlParameter
        //            {
        //                ParameterName = "@Param",
        //                DbType = System.Data.DbType.String,
        //                Size = 100,
        //                Direction = System.Data.ParameterDirection.Output
        //            };

        //            var salary = await _context.Salaries.FromSqlRaw("dbo.SP_salary @y, @m, @Param OUT", y, m, outParam).ToListAsync();
        //            var emp = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();

        //            salary.ForEach(i => {
        //                emp.ForEach(j => { if (i.Employee == j.Id) i.EmployeeNavigation.Names = j.Names; });
        //           });

        //            var budget = await _context.Budgets.FromSqlRaw("dbo.indexBudget").ToListAsync();
        //            if (outParam.SqlValue.ToString() == "1")
        //            {
        //                ViewBag.Message = "This month you have already issued a salary!";
        //                ViewBag.salary = null;
        //                ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
        //                return View();
        //            }
        //            else if (outParam.SqlValue.ToString() == "0")
        //            {
        //                ViewBag.salary = salary;
        //                decimal? tot = 0;
        //                salary.ForEach(i => { tot += i.TotalAmount; });
        //                ViewBag.total = tot;
        //                ViewBag.budget = budget.FirstOrDefault().SumOfBudget;
        //                ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
        //                return View();
        //            }
        //            else if (outParam.SqlValue.ToString() != null)
        //            {
        //                decimal? total = 0;
        //                salary.ForEach(i => { total += i.TotalAmount; });
        //                ViewBag.total = total;
        //                ViewBag.budget = budget.FirstOrDefault().SumOfBudget;
        //                ViewBag.salary = salary;
        //                await _context.SaveChangesAsync();
        //                //return RedirectToAction(nameof(Index));
        //                ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
        //                return View();
        //            }
        //        }
        //        catch (SqlException ex)
        //        {
        //            return NotFound(ex.Message);
        //        }
        //       }
        //    ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
        //    return View();
        //}

        private List<Salary> rachet()
        {

            var emp = _context.Employees.ToList();
            var budget = _context.Budgets.Where(i => i.Id == 1002).FirstOrDefault();
            List<Salary> salaries = new List<Salary>(emp.Count());

            ////////////////////////////////purchase 
            var purchase =  _context.PurchaseOfRawMaterials.ToList();
            List<int?> amountInPurchase = new List<int?>();
            int count = 0;
            List<decimal?> bonus = new List<decimal?>();

            emp.ForEach(i =>
            {
                purchase.ForEach(j => { if (i.Id == j.Employee) { if (j.Datee.Value.Year == DateTime.Now.Year && j.Datee.Value.Month == DateTime.Now.Month) { count++; } } }); amountInPurchase.Add(count);
                bonus.Add(count * budget.Bonus * i.Salary / 100);
                count = 0;
            });

            ////////////////////////////////sale
            int k = 0;
            var sale = _context.SaleOfProducts.ToList();
            List<int?> amountInSale = new List<int?>();
            emp.ForEach(i =>
            {
                sale.ForEach(j => { if (i.Id == j.Employee) { if (j.Datee.Value.Year == DateTime.Now.Year && j.Datee.Value.Month == DateTime.Now.Month) { count++; } } }); amountInSale.Add(count);
                bonus[k] += count * budget.Bonus * i.Salary / 100;
                k++;
                count = 0;
            });


            ////////////////////////////////production
            k = 0;
            var production = _context.Productions.ToList();
            List<int?> amountInProduction = new List<int?>();
            emp.ForEach(i =>
            {
                production.ForEach(j => { if (i.Id == j.Employee) { if (j.Datee.Value.Year == DateTime.Now.Year && j.Datee.Value.Month == DateTime.Now.Month) { count++; } } }); amountInProduction.Add(count);
                bonus[k] += count * budget.Bonus * i.Salary / 100;
                k++;
                count = 0;
            });

            k = 0;
            emp.ForEach(i =>
            {
                salaries.Add(new Salary
                {
                    Date = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    Employee = i.Id,
                    Purchase = amountInPurchase[k],
                    Sale = amountInSale[k],
                    Production = amountInProduction[k],
                    Count = amountInPurchase[k] + amountInSale[k] + amountInProduction[k],
                    Bonus = bonus[k],
                    Salary1 = i.Salary,
                    TotalAmount = i.Salary + bonus[k],
                    Issued = false
                });
                k++;
            });

            return salaries;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int Date, int Month)
        {
            if (ModelState.IsValid)
            {
                var emp = await _context.Employees.ToListAsync();
                List<Salary> salaries = new List<Salary>(emp.Count());
                var budget = await _context.Budgets.Where(i => i.Id == 1002).FirstOrDefaultAsync();
                var contextSalary = await _context.Salaries.ToListAsync();
                foreach (var i in contextSalary)
                {
                    if (i.Date == Date && i.Month == Month && i.Issued == true)
                    {
                        ViewBag.Message = "This month you have already issued a salary!";
                        ViewBag.salary = null;
                        ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
                        return View();
                    }
                    else if (i.Date == Date && i.Month == Month && i.Issued == false)
                    {
                        bool val = false;       
                        SqlParameter y = new SqlParameter("@y", Date);
                        SqlParameter m = new SqlParameter("@m", Month);
                        SqlParameter isSued = new SqlParameter("@isSued", i.Issued);

                        await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteS @y, @m, @isSued", y, m, isSued);

                        List<Salary> salaries1 = rachet();
                        salaries1.ForEach(i => _context.Add(i));
                        ViewBag.salary = salaries1;
                        decimal? tot = 0;
                        salaries1.ForEach(i => { tot += i.TotalAmount; });
                        await _context.SaveChangesAsync();

                        ViewBag.total = tot;
                        ViewBag.budget = budget.SumOfBudget;
                        ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
                        return View();
                    }
                }

                var sal = rachet();
                decimal? total = 0;
                sal.ForEach(i => { salary.Add(i); });
                sal.ForEach(i => { total += i.TotalAmount; });
                ViewBag.total = total;
                ViewBag.budget = budget.SumOfBudget;
                ViewBag.salary = sal;
                sal.ForEach(i => { _context.Add(i); });
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
                return View();

            }
            ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
            return View();
        }


        //public async Task<IActionResult> paySalary()
        //{
        //    DateTime dateTime = DateTime.Now;

        //    SqlParameter y = new SqlParameter("@y", dateTime.Year);
        //    SqlParameter m = new SqlParameter("@m", dateTime.Month);

        //    var outParam = new SqlParameter
        //    {
        //        ParameterName = "@Param",
        //        DbType = System.Data.DbType.String,
        //        Size = 100,
        //        Direction = System.Data.ParameterDirection.Output
        //    };

        //    var salary = await _context.Salaries.FromSqlRaw("dbo.paySalary @y, @m, @Param OUT", y, m, outParam).ToListAsync();
        //    var emp = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();

        //    salary.ForEach(i => {
        //        emp.ForEach(j => { if (i.Employee == j.Id) i.EmployeeNavigation.Names = j.Names; });
        //    });

        //    var budget = await _context.Budgets.FromSqlRaw("dbo.indexBudget").ToListAsync();

        //    if (outParam.SqlValue.ToString()=="0")
        //    {
        //        ViewBag.Message = "Not enough budget!";
        //        ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
        //        return View();
        //    }
        //    else if(outParam.SqlValue.ToString()=="1")
        //    {
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
        //    return View();
        //}

        public async Task<IActionResult> paySalary()
        {
            DateTime dateTime = DateTime.Now;
            var budget = await _context.Budgets.Where(i => i.Id == 1002).FirstOrDefaultAsync();
            var salary = await _context.Salaries.Where(i => i.Date == dateTime.Year && i.Month == dateTime.Month && i.Issued == false).ToListAsync();
            decimal? total = 0;
            salary.ForEach(i => { total += i.TotalAmount; });

            if (total <= budget.SumOfBudget)
            {
                salary.ForEach(i => { i.Issued = true; });
                budget.SumOfBudget -= total;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Message = "Not enough budget!";
                ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
                return View();
            }
            ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1");
            return View();
        }

        static decimal? oldTotalAmount;
        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Salaries == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries.FindAsync(id);
            oldTotalAmount = salary.TotalAmount ;
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names", salary.Employee);
            ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1", salary.Month);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Month,Employee,Purchase,Sale,Production,Count,Salary1,TotalAmount,Issued,Bonus")] Salary salary)
        {
            if (id != salary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var budget = await _context.Budgets.Where(b => b.Id == 1002).FirstOrDefaultAsync();
                    decimal? bonus = 0;
                    ///////////////////purchase
                    int inPurchaseAmount = 0;
                    decimal? inPurchaseSumma = 0;
                    DateTime year = Convert.ToDateTime(salary.Date);
                    var purchase = await _context.PurchaseOfRawMaterials.Where(p => p.Employee == salary.Employee).ToListAsync();
                    inPurchaseAmount = purchase.Count();
                    purchase.ForEach(i => { inPurchaseSumma += i.Summa; });
                    bonus += inPurchaseSumma / 100 * budget.Bonus;

                    ///////////////////sale
                    int inSaleAmount = 0;
                    decimal? inSaleSumma = 0;

                    var sale = await _context.SaleOfProducts.Where(s => s.Employee == salary.Employee).ToListAsync();
                    inSaleAmount = sale.Count();
                    sale.ForEach(i => { inSaleSumma += i.Summa; });
                    bonus += inSaleSumma / 100 * budget.Bonus;

                    ///////////////////production
                    int inProductionAmount = 0;
                    decimal? inProductionSumma = 0;

                    var production = await _context.Productions.Where(p => p.Employee == salary.Employee).ToListAsync();
                    var finProd = await _context.FinishedProducts.ToListAsync();
                    List<FinishedProduct> finProdFilterOn = new List<FinishedProduct>(production.Count());
                    finProd.ForEach(i => { production.ForEach(j => { if (i.Id == j.Product) { finProdFilterOn.Add(i); } }); });
                    inProductionAmount = production.Count();

                    List<decimal?> finProdCost = new List<decimal?>(finProdFilterOn.Count());
                    finProdFilterOn.ForEach(i => { finProdCost.Add(i.Summa / i.Amount); });

                    List<decimal?> prodSum = new List<decimal?>(finProdCost.Count());
                    for (var i = 0; i < production.Count(); prodSum.Add(production[i].Amount * finProdCost[i]), i++) ;
                    prodSum.ForEach(i => { inProductionSumma += i; });
                    bonus += inProductionSumma / 100 * budget.Bonus;

                    salary.Purchase = inPurchaseAmount;
                    salary.Sale = inSaleAmount;
                    salary.Production = inProductionAmount;
                    salary.Count = inPurchaseAmount + inSaleAmount + inProductionAmount;

                    var emp = await _context.Employees.Where(e => e.Id == salary.Employee).FirstOrDefaultAsync();
                    salary.Salary1 = emp.Salary;
                    salary.TotalAmount = salary.Salary1 + bonus;
                    salary.Bonus = bonus;

                    if ((bool)salary.Issued)
                    {
                        if (salary.TotalAmount > budget.SumOfBudget + oldTotalAmount)
                        {
                            ViewBag.Message = "Not enouhg budget";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names", salary.Employee);
                            ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1", salary.Month);
                            return View();
                        }
                        else
                        {
                            budget.SumOfBudget += oldTotalAmount;
                            budget.SumOfBudget -= salary.TotalAmount;
                            _context.Update(salary);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        if (salary.TotalAmount > budget.SumOfBudget + oldTotalAmount) 
                        { 
                            ViewBag.Message = "Not enouhg budget";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names", salary.Employee);
                            ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1", salary.Month);
                            return View();
                        }
                        else
                        {
                            budget.SumOfBudget -= oldTotalAmount;
                            budget.SumOfBudget += salary.TotalAmount;
                            _context.Update(salary);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(salary.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names", salary.Employee);
            ViewData["Month"] = new SelectList(_context.Months, "Id", "Month1", salary.Month);
            return View(salary);
        }

        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Salaries == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.EmployeeNavigation)
                .Include(s => s.MonthNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Salaries == null)
            {
                return Problem("Entity set 'DBaseContext.Salaries'  is null.");
            }
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
          return (_context.Salaries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
