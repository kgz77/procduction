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
    public class SaleOfProductsController : Controller
    {
        private readonly DBaseContext _context;

        public SaleOfProductsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: SaleOfProducts
        public async Task<IActionResult> Index(int pg = 1)
        {
            var saleOfProducts = await _context.SaleOfProducts.FromSqlRaw("dbo.indexSaleOfProduct").ToListAsync();
            var dataProduct = await _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToListAsync();
            var dataEmploye = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();

            foreach (var sale in saleOfProducts)
            {
                foreach (var prod in dataProduct)
                {
                    if (sale.Product == prod.Id)
                    {
                        sale.ProductNavigation.Title = prod.Title;
                    }
                }
                foreach (var emp in dataEmploye)
                {
                    if (sale.Employee == emp.Id)
                    {
                        sale.EmployeeNavigation.Names = emp.Names;
                    }
                }
            }
            //IQueryable<SaleOfProduct> saleOfProducts = _context.SaleOfProducts.Include(s => s.EmployeeNavigation).Include(s => s.ProductNavigation);
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = saleOfProducts.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<SaleOfProduct> data = saleOfProducts.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: SaleOfProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var saleOfProduct = await _context.SaleOfProducts
            //    .Include(s => s.EmployeeNavigation)
            //    .Include(s => s.ProductNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var saleOfProduct = await _context.SaleOfProducts.FromSqlRaw("dbo.selectByIdSaleOfProduct @Id", Id).ToListAsync();

            SqlParameter IdProd = new SqlParameter("@IdProd", saleOfProduct.FirstOrDefault().Product);
            var products = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @IdProd", IdProd).ToListAsync();

            SqlParameter IdEmp = new SqlParameter("@IdEmp", saleOfProduct.FirstOrDefault().Employee);
            var employees = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @IdEmp", IdEmp).ToListAsync();

            if (saleOfProduct.FirstOrDefault().Product == products.FirstOrDefault().Id)
                saleOfProduct.FirstOrDefault().ProductNavigation.Title = products.FirstOrDefault().Title;
            if (saleOfProduct.FirstOrDefault().Employee == employees.FirstOrDefault().Id)
                saleOfProduct.FirstOrDefault().EmployeeNavigation.Names = employees.FirstOrDefault().Names;

            if (saleOfProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(saleOfProduct.FirstOrDefault());
        }

        // GET: SaleOfProducts/Create
        public IActionResult Create()
        {
            var dataProduct = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();

            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
            ViewData["Product"] = new SelectList(dataProduct, "Id", "Title");
            return View();
        }

        // POST: SaleOfProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Product,Amount,Summa,Datee,Employee")] SaleOfProduct saleOfProduct)
        {
            var dataProduct = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            if (ModelState.IsValid)
            {
                //List<FinishedProduct> finishedProducts = await _context.FinishedProducts.ToListAsync();
                //List<Budget> budgets = await _context.Budgets.ToListAsync();
                //DateTime dateNow = DateTime.Now;

                //foreach (var item in finishedProducts)
                //{
                //    if (item.Id == saleOfProduct.Product)
                //    {
                //        if(item.Amount < saleOfProduct.Amount)
                //        {
                //            return NotFound("Not enough product!");
                //        }
                //        else if(item.Summa < saleOfProduct.Summa)
                //        {
                //            return NotFound("Not enough sum!");
                //        }
                //        else if(dateNow < saleOfProduct.Datee || saleOfProduct.Datee == null)
                //        {
                //            return NotFound("The date is wrong!");
                //        }
                //        else if(saleOfProduct.Amount< 0 || saleOfProduct.Summa < 0)
                //        {
                //            return NotFound("The number and amount cannot be negative!");
                //        }
                //        else
                //        {
                //            var percent = budgets[0].PercentageOfPremium;
                //            budgets[0].SumOfBudget += (saleOfProduct.Summa * percent / 100) + saleOfProduct.Summa;
                //            item.Amount -= saleOfProduct.Amount;
                //            item.Summa -= saleOfProduct.Summa;
                //        }
                //    }
                //}
                //_context.Add(saleOfProduct);
                //await _context.SaveChangesAsync();

                try
                {

                    var sal = await _context.Salaries.Where(i => i.Issued == true).ToListAsync();

                    foreach (var i in sal)
                    {
                        if (i.Date == saleOfProduct.Datee.Value.Year && i.Month == saleOfProduct.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names");
                            ViewData["RawMaterial"] = new SelectList(_context.RawMaterials, "Id", "Title");
                            return View();
                        }
                    }

                    SqlParameter Product = new SqlParameter("@Product", saleOfProduct.Product);
                    SqlParameter Amount = new SqlParameter("@Amount", saleOfProduct.Amount);
                    SqlParameter Summa = new SqlParameter("@Summa", saleOfProduct.Summa);
                    SqlParameter Datee = new SqlParameter("@Datee", saleOfProduct.Datee);
                    SqlParameter Employee = new SqlParameter("@Employee", saleOfProduct.Employee);
                    var outParam = new SqlParameter
                    {
                        ParameterName = "@Param",
                        DbType = System.Data.DbType.String,
                        Size = 100,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createSaleOfProduct @Product, @Amount, @Summa, @Datee, @Employee, @Param OUT", Product, Amount, Summa, Datee, Employee, outParam);
                    if (outParam.SqlValue.ToString() == "Null")
                        return RedirectToAction(nameof(Index));

                    if (outParam.SqlValue.ToString() != null)
                    {
                        ViewBag.Message = outParam.SqlValue.ToString();
                        ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
                        ViewData["Product"] = new SelectList(dataProduct, "Id", "Title");
                        return View();
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }
            }
           
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names", saleOfProduct.Employee);
            ViewData["Product"] = new SelectList(dataProduct, "Id", "Title", saleOfProduct.Product);
            return View(saleOfProduct);
        }

        static decimal sumOfSale = 0;
        static decimal amounOfSale = 0;
        // GET: SaleOfProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var saleOfProduct = await _context.SaleOfProducts.FromSqlRaw("dbo.selectByIdSaleOfProduct @Id", Id).ToListAsync();

            //var saleOfProduct = await _context.SaleOfProducts.FindAsync(id);
            //sumOfSale = (decimal)saleOfProduct.Summa;
            //amounOfSale = (decimal)saleOfProduct.Amount;
            if (saleOfProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }
            var dataProduct = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            ViewData["Employee"] = new SelectList(dataEmploye.Where(i=>i.Id==saleOfProduct.FirstOrDefault().Employee), "Id", "Names", saleOfProduct.FirstOrDefault().Employee);
            ViewData["Product"] = new SelectList(dataProduct.Where(i=>i.Id==saleOfProduct.FirstOrDefault().Product), "Id", "Title", saleOfProduct.FirstOrDefault().Product);
            return View(saleOfProduct.FirstOrDefault());
        }

        // POST: SaleOfProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaleOfProduct saleOfProduct)
        {
            if (id != saleOfProduct.Id)
            {
                return NotFound();
            }
            var dataProduct = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            if (ModelState.IsValid)
            {
                //List<FinishedProduct> finishedProducts = await _context.FinishedProducts.ToListAsync();
                //List<Budget> budgets = await _context.Budgets.ToListAsync();
                //DateTime dateNow = DateTime.Now;

                //foreach (var item in finishedProducts)
                //{
                //    if (item.Id == saleOfProduct.Product)
                //    {
                //        var amountChekck = item.Amount + amounOfSale;
                //        var sumCheck = item.Summa + sumOfSale;
                //        if (amountChekck < saleOfProduct.Amount)
                //        {
                //            return NotFound("Not enough product!");
                //        }
                //        else if (sumCheck < saleOfProduct.Summa)
                //        {
                //            return NotFound("Not enough sum!");
                //        }
                //        else if (dateNow < saleOfProduct.Datee || saleOfProduct.Datee == null)
                //        {
                //            return NotFound("The date is wrong!");
                //        }
                //        else if (saleOfProduct.Amount < 0 || saleOfProduct.Summa < 0)
                //        {
                //            return NotFound("The number and amount cannot be negative!");
                //        }
                //        else
                //        {
                //            var percent = budgets[0].PercentageOfPremium;
                //            var valueSum = (decimal)(sumOfSale + (sumOfSale * percent / 100));
                //            budgets[0].SumOfBudget -= valueSum; 
                //            budgets[0].SumOfBudget += (saleOfProduct.Summa * percent / 100) + saleOfProduct.Summa;
                //            item.Amount += (decimal)amounOfSale;
                //            item.Amount -= saleOfProduct.Amount;
                //            item.Summa += (decimal)sumOfSale;
                //            item.Summa -= (decimal)saleOfProduct.Summa; 
                //        }
                //    }
                //}
                //_context.Update(saleOfProduct);
                //await _context.SaveChangesAsync();
                try
                {
                    var sal = await _context.Salaries.Where(i => i.Issued == true).ToListAsync();

                    foreach (var i in sal)
                    {
                        if (i.Date == saleOfProduct.Datee.Value.Year && i.Month == saleOfProduct.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names");
                            ViewData["RawMaterial"] = new SelectList(_context.RawMaterials, "Id", "Title");
                            return View();
                        }
                    }

                    SqlParameter Id = new SqlParameter("@Id", saleOfProduct.Id);
                    SqlParameter Product = new SqlParameter("@Product", saleOfProduct.Product);
                    SqlParameter Amount = new SqlParameter("@Amount", saleOfProduct.Amount);
                    SqlParameter Summa = new SqlParameter("@Summa", saleOfProduct.Summa);
                    SqlParameter Datee = new SqlParameter("@Datee", saleOfProduct.Datee);
                    SqlParameter Employee = new SqlParameter("@Employee", saleOfProduct.Employee);
                    var outParam = new SqlParameter
                    {
                        ParameterName = "@Param",
                        DbType = System.Data.DbType.String,
                        Size = 100,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editSaleOfProduct @Id, @Product, @Amount, @Summa, @Datee, @Employee, @Param OUT",Id, Product, Amount, Summa, Datee, Employee, outParam);
                    if (outParam.SqlValue.ToString() == "Null")
                        return RedirectToAction(nameof(Index));

                    if (outParam.SqlValue.ToString() != null)
                    {
                        ViewBag.Message = outParam.SqlValue.ToString();
                        ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
                        ViewData["Product"] = new SelectList(dataProduct, "Id", "Title");
                        return View();
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }
            }
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names", saleOfProduct.Employee);
            ViewData["Product"] = new SelectList(dataProduct, "Id", "Title", saleOfProduct.Product);
            return View(saleOfProduct);
        }

        // GET: SaleOfProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var saleOfProduct = await _context.SaleOfProducts
            //    .Include(s => s.EmployeeNavigation)
            //    .Include(s => s.ProductNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var saleOfProduct = await _context.SaleOfProducts.FromSqlRaw("dbo.selectByIdSaleOfProduct @Id", Id).ToListAsync();

            SqlParameter IdProd = new SqlParameter("@IdProd", saleOfProduct.FirstOrDefault().Product);
            var products = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @IdProd", IdProd).ToListAsync();

            SqlParameter IdEmp = new SqlParameter("@IdEmp", saleOfProduct.FirstOrDefault().Employee);
            var employees = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @IdEmp", IdEmp).ToListAsync();

            if (saleOfProduct.FirstOrDefault().Product == products.FirstOrDefault().Id)
                saleOfProduct.FirstOrDefault().ProductNavigation.Title = products.FirstOrDefault().Title;
            if (saleOfProduct.FirstOrDefault().Employee == employees.FirstOrDefault().Id)
                saleOfProduct.FirstOrDefault().EmployeeNavigation.Names = employees.FirstOrDefault().Names;

            if (saleOfProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(saleOfProduct.FirstOrDefault());
        }

        // POST: SaleOfProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var saleOfProduct = await _context.SaleOfProducts.FindAsync(id);
            //List<FinishedProduct> finishedProducts = await _context.FinishedProducts.Include(f => f.UnitNavigation).ToListAsync();
            //List<Budget> budgets = await _context.Budgets.ToListAsync();

            //foreach (var item in finishedProducts)
            //{
            //    if (item.Id == saleOfProduct.Product)
            //    {
            //        var percent = budgets[0].PercentageOfPremium;
            //        budgets[0].SumOfBudget -= (saleOfProduct.Summa * percent / 100) + saleOfProduct.Summa;
            //        item.Amount += saleOfProduct.Amount;
            //        item.Summa += saleOfProduct.Summa;
            //    }
            //}
            //_context.SaleOfProducts.Remove(saleOfProduct);
            //await _context.SaveChangesAsync();
            try
            {
                SqlParameter Id = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteSaleOfProduct @Id", Id);
                return RedirectToAction(nameof(Index));
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private bool SaleOfProductExists(int id)
        {
            return _context.SaleOfProducts.Any(e => e.Id == id);
        }
    }
}
