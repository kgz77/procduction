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
    public class ProductionsController : Controller
    {
        private readonly DBaseContext _context;

        public ProductionsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: Productions
        public async Task<IActionResult> Index(int pg = 1)
        {
            
            var productions = await _context.Productions.FromSqlRaw("dbo.indexProduction").ToListAsync();
            var dataFinProd = await _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToListAsync();
            var dataEmploye = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();

            foreach (var prod in productions)
            {
                foreach (var finProd in dataFinProd)
                {
                    if (prod.Product == finProd.Id)
                    {
                        prod.ProductNavigation.Title = finProd.Title;
                    }
                }
                foreach (var emp in dataEmploye)
                {
                    if (prod.Employee == emp.Id)
                    {
                        prod.EmployeeNavigation.Names = emp.Names;
                    }
                }
            }
            //IQueryable<Production> productions = _context.Productions.Include(p => p.EmployeeNavigation).Include(p=>p.ProductNavigation);
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = productions.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<Production> data = productions.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Productions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var production = await _context.Productions.FromSqlRaw("dbo.selectByIdProduction @Id", Id).ToListAsync();

            SqlParameter IdFinProd = new SqlParameter("@IdFinProd", production.FirstOrDefault().Product);
            var finishedProducts = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdEmploye @IdFinProd", IdFinProd).ToListAsync();
            
            SqlParameter IdEmp = new SqlParameter("@IdEmp", production.FirstOrDefault().Employee);
            var employees = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @IdEmp", IdEmp).ToListAsync();
            
            if (production.FirstOrDefault().Product == finishedProducts.FirstOrDefault().Id)
                production.FirstOrDefault().ProductNavigation.Title = finishedProducts.FirstOrDefault().Title;
            if (production.FirstOrDefault().Employee == employees.FirstOrDefault().Id)
                production.FirstOrDefault().EmployeeNavigation.Names = employees.FirstOrDefault().Names;

            //var production = await _context.Productions
            //    .Include(p => p.EmployeeNavigation)
            //    .Include(p => p.ProductNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (production.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(production.FirstOrDefault());
        }

        // GET: Productions/Create
        public IActionResult Create()
        {
            var dataFinProd =  _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye =  _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
            ViewData["Product"] = new SelectList(dataFinProd, "Id", "Title");
            return View();
        }

        // POST: Productions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Product,Amount,Datee,Employee")] Production production)
        {
            //List<FinishedProduct> finishedProducts = await _context.FinishedProducts.ToListAsync();
            //List<RawMaterial> rawMaterials = await _context.RawMaterials.ToListAsync();
            //DateTime today = DateTime.Now;
            var dataFinProd = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            if (ModelState.IsValid)
            {
                //var finishedProduct = _context.FinishedProducts.Where(f => f.Id == production.Product).FirstOrDefault();
                //var ingridient = await _context.Ingredients.Where(i => i.Product == finishedProduct.Id).ToListAsync();
                //if (production.Amount < 0)
                //{
                //    ModelState.AddModelError("Amount", "The amount cannot be negative");
                //}
                //else if (production.Amount == 0)
                //{
                //    ModelState.AddModelError("Amount", "Error!");
                //}
                //else if(production.Amount == null)
                //{
                //    ModelState.AddModelError("Amount", "The field cannot be empty");
                //}
                //else if(production.Amount.GetType() == typeof(String))
                //{
                //    ModelState.AddModelError("Amount", "Enter the number!");
                //}
                //else if(production.Datee > today)
                //{
                //    ModelState.AddModelError("Datee", "The date is wrong");
                //}
                //else if (production.Datee == null)
                //{
                //    ModelState.AddModelError("Datee", "The field cannot be empty");
                //}
                //else
                //{
                //    foreach (var r in rawMaterials)
                //    {
                //        foreach (var i in ingridient)
                //        {
                //            if (r.Id == i.RawMaterial)
                //            {
                //                if (r.Amount < production.Amount * i.Amount)
                //                {
                //                    ModelState.AddModelError("Amount", "Not enough ingridients!");
                //                    ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names", production.Employee);
                //                    ViewData["Product"] = new SelectList(_context.FinishedProducts, "Id", "Title", production.Product);
                //                    return View(production);
                //                }
                //                else
                //                {
                //                    var sumOfRaw = r.Summa / r.Amount;
                //                    r.Amount -= production.Amount * i.Amount;
                //                    r.Summa -= sumOfRaw * production.Amount * i.Amount;
                //                    sumOfRaw = 0;
                //                }
                //            }
                //        }
                //    }
                //    var sum = finishedProduct.Summa / finishedProduct.Amount;
                //    finishedProduct.Amount += production.Amount;
                //    finishedProduct.Summa += sum * production.Amount;
                //    sum = 0;
                //    _context.Add(production);
                //    await _context.SaveChangesAsync();
                //    return RedirectToAction(nameof(Index));
                //}
                try
                {
                    var sal = await _context.Salaries.Where(i => i.Issued == true).ToListAsync();

                    foreach (var i in sal)
                    {
                        if (i.Date == production.Datee.Value.Year && i.Month == production.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names");
                            ViewData["RawMaterial"] = new SelectList(_context.RawMaterials, "Id", "Title");
                            return View();
                        }
                    }

                    SqlParameter Product = new SqlParameter("@Product", production.Product);
                    SqlParameter Amount = new SqlParameter("@Amount", production.Amount);
                    SqlParameter Datee = new SqlParameter("@Datee", production.Datee);
                    SqlParameter Employee = new SqlParameter("@Employee", production.Employee);
                    var outParam = new SqlParameter
                    {
                        ParameterName = "@Param",
                        DbType = System.Data.DbType.String,
                        Size = 200,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createProduction @Product, @Amount, @Datee, @Employee, @Param OUT", Product, Amount, Datee, Employee, outParam);
                    if (outParam.SqlValue.ToString() == "Null")
                        return RedirectToAction(nameof(Index));

                    if (outParam.SqlValue.ToString() != null)
                    {
                        ViewBag.Message = outParam.SqlValue.ToString();
                        ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
                        ViewData["Product"] = new SelectList(dataFinProd, "Id", "Title");
                        return View();
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }

            }
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names", production.Employee);
            ViewData["Product"] = new SelectList(dataFinProd, "Id", "Title", production.Product);
            return View(production);
        }
       static Production oldProduction;
        // GET: Productions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var production = await _context.Productions.FromSqlRaw("dbo.selectByIdProduction @Id", Id).ToListAsync();

            //var production = await _context.Productions.FindAsync(id);
            //oldProduction = production;

            if (production.FirstOrDefault() == null)
            {
                return NotFound();
            }
            var dataFinProd = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            ViewData["Employee"] = new SelectList(dataEmploye.Where(i=>i.Id==production.FirstOrDefault().Employee), "Id", "Names", production.FirstOrDefault().Employee);
            ViewData["Product"] = new SelectList(dataFinProd.Where(i=>i.Id==production.FirstOrDefault().Product), "Id", "Title", production.FirstOrDefault().Product);
            return View(production.FirstOrDefault());
        }

        // POST: Productions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product,Amount,Datee,Employee")] Production production)
        {
            if (id != production.Id)
            {
                return NotFound();
            }
            var dataFinProd = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            if (ModelState.IsValid)
            {
                //try
                //{
                //    List<RawMaterial> rawMaterials = await _context.RawMaterials.ToListAsync();
                //    var finishedProduct = _context.FinishedProducts.Where(f => f.Id == production.Product).FirstOrDefault();
                //    var ingridient = await _context.Ingredients.Where(i => i.Product == finishedProduct.Id).ToListAsync();

                //    foreach (var r in rawMaterials)
                //    {
                //        foreach (var i in ingridient)
                //        {
                //            if (r.Id == i.RawMaterial)
                //            {

                //                if (r.Amount == 0)
                //                {
                //                    r.Amount += oldProduction.Amount * i.Amount;
                //                    r.Summa += oldProduction.Amount * i.Amount * r.Amount;
                //                }
                //                else
                //                {
                //                    var sumOfRaw = r.Summa / r.Amount;
                //                    r.Amount += oldProduction.Amount * i.Amount;
                //                    r.Summa += sumOfRaw * oldProduction.Amount * i.Amount;
                //                    sumOfRaw = 0;
                //                }
                //            }
                //        }
                //    }
                //    var sum = finishedProduct.Summa / finishedProduct.Amount;
                //    finishedProduct.Amount -= oldProduction.Amount;
                //    finishedProduct.Summa -= sum * oldProduction.Amount;
                //    sum = 0;

                //    //--------------------Create--------------------------
                //    DateTime today = DateTime.Now;

                //    if (production.Amount < 0)
                //    {
                //        ModelState.AddModelError("Amount", "The amount cannot be negative");
                //    }
                //    else if (production.Amount == 0)
                //    {
                //        ModelState.AddModelError("Amount", "Error!");
                //    }
                //    else if (production.Amount == null)
                //    {
                //        ModelState.AddModelError("Amount", "The field cannot be empty");
                //    }
                //    else if (production.Amount.GetType() == typeof(String))
                //    {
                //        ModelState.AddModelError("Amount", "Enter the number!");
                //    }
                //    else if (production.Datee > today)
                //    {
                //        ModelState.AddModelError("Datee", "The date is wrong");
                //    }
                //    else if (production.Datee == null)
                //    {
                //        ModelState.AddModelError("Datee", "The field cannot be empty");
                //    }
                //    else
                //    {
                //        foreach (var r in rawMaterials)
                //        {
                //            foreach (var i in ingridient)
                //            {
                //                if (r.Id == i.RawMaterial)
                //                {
                //                    if (r.Amount < production.Amount * i.Amount)
                //                    {
                //                        ModelState.AddModelError("Amount", "Not enough ingridients!");
                //                        ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names", production.Employee);
                //                        ViewData["Product"] = new SelectList(_context.FinishedProducts, "Id", "Title", production.Product);
                //                        return View(production);
                //                    }
                //                    else
                //                    {
                //                        var sumOfRaw = r.Summa / r.Amount;
                //                        r.Amount -= production.Amount * i.Amount;
                //                        r.Summa -= sumOfRaw * production.Amount * i.Amount;
                //                        sumOfRaw = 0;
                //                    }
                //                }
                //            }
                //        }
                //        var sum1 = finishedProduct.Summa / finishedProduct.Amount;
                //        finishedProduct.Amount += production.Amount;
                //        finishedProduct.Summa += sum1 * production.Amount;
                //        sum1 = 0;

                //        _context.Update(production);
                //        await _context.SaveChangesAsync();
                //    }
                //}
                //catch (DbUpdateConcurrencyException)
                //{
                //    if (!ProductionExists(production.Id))
                //    {
                //        return NotFound();
                //    }
                //    else
                //    {
                //        throw;
                //    }
                //}
                //return RedirectToAction(nameof(Index));
                try
                {
                    var sal = await _context.Salaries.Where(i => i.Issued == true).ToListAsync();

                    foreach (var i in sal)
                    {
                        if (i.Date == production.Datee.Value.Year && i.Month == production.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names");
                            ViewData["RawMaterial"] = new SelectList(_context.RawMaterials, "Id", "Title");
                            return View();
                        }
                    }

                    SqlParameter Id = new SqlParameter("@Id", production.Id);
                    SqlParameter Product = new SqlParameter("@Product", production.Product);
                    SqlParameter Amount = new SqlParameter("@Amount", production.Amount);
                    SqlParameter Datee = new SqlParameter("@Datee", production.Datee);
                    SqlParameter Employee = new SqlParameter("@Employee", production.Employee);
                    var outParam = new SqlParameter
                    {
                        ParameterName = "@Param",
                        DbType = System.Data.DbType.String,
                        Size = 200,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editProduction @Id, @Product, @Amount, @Datee, @Employee, @Param OUT",Id, Product, Amount, Datee, Employee, outParam);
                    if (outParam.SqlValue.ToString() == "Null")
                        return RedirectToAction(nameof(Index));

                    if (outParam.SqlValue.ToString() != null)
                    {
                        ViewBag.Message = outParam.SqlValue.ToString();
                        ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
                        ViewData["Product"] = new SelectList(dataFinProd, "Id", "Title");
                        return View();
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }

            }
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names", production.Employee);
            ViewData["Product"] = new SelectList(dataFinProd, "Id", "Title", production.Product);
            return View(production);
        }

        // GET: Productions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var production = await _context.Productions
            //    .Include(p => p.EmployeeNavigation)
            //    .Include(p => p.ProductNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            SqlParameter Id = new SqlParameter("@Id", id);
            var production = await _context.Productions.FromSqlRaw("dbo.selectByIdProduction @Id", Id).ToListAsync();

            SqlParameter IdFinProd = new SqlParameter("@IdFinProd", production.FirstOrDefault().Product);
            var finishedProducts = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @IdFinProd", IdFinProd).ToListAsync();

            SqlParameter IdEmp = new SqlParameter("@IdEmp", production.FirstOrDefault().Employee);
            var employees = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @IdEmp", IdEmp).ToListAsync();

            if (production.FirstOrDefault().Product == finishedProducts.FirstOrDefault().Id)
                production.FirstOrDefault().ProductNavigation.Title = finishedProducts.FirstOrDefault().Title;
            if (production.FirstOrDefault().Employee == employees.FirstOrDefault().Id)
                production.FirstOrDefault().EmployeeNavigation.Names = employees.FirstOrDefault().Names;

            if (production.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(production.FirstOrDefault());
        }

        // POST: Productions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var production = await _context.Productions.FindAsync(id);
            //List<RawMaterial> rawMaterials = await _context.RawMaterials.ToListAsync();
            //var finishedProduct = _context.FinishedProducts.Where(f => f.Id == production.Product).FirstOrDefault();
            //var ingridient = await _context.Ingredients.Where(i => i.Product == finishedProduct.Id).ToListAsync();

            //foreach (var r in rawMaterials)
            //{
            //    foreach (var i in ingridient)
            //    {
            //        if (r.Id == i.RawMaterial)
            //        {

            //            if (r.Amount == 0)
            //            {
            //                r.Amount += production.Amount * i.Amount;
            //                r.Summa += production.Amount * i.Amount * r.Amount;
            //            }
            //            else
            //            {
            //                var sumOfRaw = r.Summa / r.Amount;
            //                r.Amount += production.Amount * i.Amount;
            //                r.Summa += sumOfRaw * production.Amount * i.Amount;
            //                sumOfRaw = 0;
            //            }
            //        }
            //    }
            //}
            //var sum = finishedProduct.Summa / finishedProduct.Amount;
            //finishedProduct.Amount -= production.Amount;
            //finishedProduct.Summa -= sum * production.Amount;
            //sum = 0;
            //_context.Productions.Remove(production);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            try
            {
                SqlParameter Id = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteProduction @Id", Id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }

        }

        private bool ProductionExists(int id)
        {
            return _context.Productions.Any(e => e.Id == id);
        }
    }
}
