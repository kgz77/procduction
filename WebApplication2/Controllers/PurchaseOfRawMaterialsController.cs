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
    public class PurchaseOfRawMaterialsController : Controller
    {
        private readonly DBaseContext _context;

        public PurchaseOfRawMaterialsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: PurchaseOfRawMaterials
        public async Task<IActionResult> Index(int pg = 1)
        {
            var purchaseOfRawMaterials = await _context.PurchaseOfRawMaterials.FromSqlRaw("dbo.indexPurchaseOfRawMaterial").ToListAsync();
            var dataRawMaterial = await _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToListAsync();
            var dataEmploye = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();

            foreach(var pur in purchaseOfRawMaterials)
            {
                foreach(var raw in dataRawMaterial)
                {
                    if (pur.RawMaterial == raw.Id)
                    {
                        pur.RawMaterialNavigation.Title = raw.Title;
                    }
                }
                foreach(var emp in dataEmploye)
                {
                    if(pur.Employee == emp.Id)
                    {
                        pur.EmployeeNavigation.Names = emp.Names;
                    }
                }
            }

            //IQueryable<PurchaseOfRawMaterial> purchaseOfRawMaterials = _context.PurchaseOfRawMaterials.Include(p => p.EmployeeNavigation).Include(p => p.RawMaterialNavigation);
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = purchaseOfRawMaterials.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<PurchaseOfRawMaterial> data = purchaseOfRawMaterials.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: PurchaseOfRawMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var purchaseOfRawMaterials = await _context.PurchaseOfRawMaterials.FromSqlRaw("dbo.selectByIdPurchaseOfRawMaterial @Id", Id).ToListAsync();

            SqlParameter IdRaw = new SqlParameter("@IdRaw", purchaseOfRawMaterials.FirstOrDefault().RawMaterial);
            var rawMaterials = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @IdRaw", IdRaw).ToListAsync();

            SqlParameter IdEmp = new SqlParameter("@IdEmp", purchaseOfRawMaterials.FirstOrDefault().Employee);
            var employees = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @IdEmp", IdEmp).ToListAsync();

            if (purchaseOfRawMaterials.FirstOrDefault().RawMaterial == rawMaterials.FirstOrDefault().Id)
                purchaseOfRawMaterials.FirstOrDefault().RawMaterialNavigation.Title = rawMaterials.FirstOrDefault().Title;
            if (purchaseOfRawMaterials.FirstOrDefault().Employee == employees.FirstOrDefault().Id)
                purchaseOfRawMaterials.FirstOrDefault().EmployeeNavigation.Names = employees.FirstOrDefault().Names;
            //var purchaseOfRawMaterial = await _context.PurchaseOfRawMaterials
            //    .Include(p => p.EmployeeNavigation)
            //    .Include(p => p.RawMaterialNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOfRawMaterials.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(purchaseOfRawMaterials.FirstOrDefault());
        }

        // GET: PurchaseOfRawMaterials/Create
        public IActionResult Create()
        {
            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title");
            return View();
        }

        // POST: PurchaseOfRawMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RawMaterial,Amount,Summa,Datee,Employee")] PurchaseOfRawMaterial purchaseOfRawMaterial)
        {
            //List<Budget> budgets = _context.Budgets.ToList();
            //var sumOfbudget = budgets[0].SumOfBudget;
            //DateTime dateNow = DateTime.Now;
            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            if (ModelState.IsValid)
            {
                /*List<PurchaseOfRawMaterial> purchaseOfRawMaterials = _context.PurchaseOfRawMaterials.ToList();
                List<RawMaterial> rawMaterials = _context.RawMaterials.ToList();
                if (purchaseOfRawMaterial.Summa > budgets[0].SumOfBudget)
                {
                    ModelState.AddModelError("Summa", "Not enough budget");
                }
                else if (purchaseOfRawMaterial.Summa < 0)
                {
                    ModelState.AddModelError("Summa", "The sum cannot be negative");
                }
                else if (purchaseOfRawMaterial.Summa == 0)
                {
                    ModelState.AddModelError("Summa", "The sum cannot be 0");
                }
                else if (purchaseOfRawMaterial.Summa == null)
                {
                    ModelState.AddModelError("Summa", "The field cannot be empty");
                }
                else if (purchaseOfRawMaterial.Amount == null)
                {
                    ModelState.AddModelError("Amount", "The field cannot be empty");
                }
                else if (purchaseOfRawMaterial.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "The amount cannot be 0");
                }
                else if (purchaseOfRawMaterial.Amount < 0)
                {
                    ModelState.AddModelError("Amount", "The amount cannot be negative");
                }
                else if (purchaseOfRawMaterial.Datee > dateNow)
                {
                    ModelState.AddModelError("Datee", "The date is wrong");
                }
                else if (purchaseOfRawMaterial.Datee == null)
                {
                    ModelState.AddModelError("Datee", "The field cannot be empty");
                }
                else
                {
                    budgets[0].SumOfBudget -= purchaseOfRawMaterial.Summa;
                    foreach (var item in rawMaterials)
                    {
                        if (item.Id == purchaseOfRawMaterial.RawMaterial)
                        {
                            item.Amount = purchaseOfRawMaterial.Amount + item.Amount;
                            item.Summa = purchaseOfRawMaterial.Summa + item.Summa;
                        }
                    }
                    _context.Add(purchaseOfRawMaterial);
                    await _context.SaveChangesAsync();
                }*/
                try
                {
                    var sal = await _context.Salaries.Where(i => i.Issued == true).ToListAsync();

                    foreach(var i in sal)
                    {
                        if(i.Date == purchaseOfRawMaterial.Datee.Value.Year && i.Month == purchaseOfRawMaterial.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
                            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title");
                            return View();
                        }
                    }

                    SqlParameter RawMaterial = new SqlParameter("@RawMaterial", purchaseOfRawMaterial.RawMaterial);
                    SqlParameter Amount = new SqlParameter("@Amount", purchaseOfRawMaterial.Amount);
                    SqlParameter Summa = new SqlParameter("@Summa", purchaseOfRawMaterial.Summa);
                    SqlParameter Datee = new SqlParameter("@Datee", purchaseOfRawMaterial.Datee);
                    SqlParameter Employee = new SqlParameter("@Employee", purchaseOfRawMaterial.Employee);
                    var outParam = new SqlParameter
                    {
                        ParameterName = "@Param",
                        DbType = System.Data.DbType.String,
                        Size = 100,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createPurchaseOfRawMaterial @RawMaterial, @Amount, @Summa, @Datee, @Employee, @Param OUT", RawMaterial, Amount, Summa, Datee, Employee, outParam);
                    if (outParam.SqlValue.ToString() == "Null")
                        return RedirectToAction(nameof(Index));

                    if (outParam.SqlValue.ToString() != null)
                    {
                        ViewBag.Message = outParam.SqlValue.ToString();
                        ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names");
                        ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title");
                        return View();
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }

            }
            //var rawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();
            //var employe = _context.Posts.FromSqlRaw("dbo.indexPost").ToList();

            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names", purchaseOfRawMaterial.Employee);
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", purchaseOfRawMaterial.RawMaterial);
            return View(purchaseOfRawMaterial);
        }
        static decimal sumOfPurchase = 0;
        static decimal amounOfPurchase = 0;
        static PurchaseOfRawMaterial p = null;
            // GET: PurchaseOfRawMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {   
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var purchaseOfRawMaterials = await _context.PurchaseOfRawMaterials.FromSqlRaw("dbo.selectByIdPurchaseOfRawMaterial @Id", Id).ToListAsync();
            p = purchaseOfRawMaterials.FirstOrDefault();
            //var purchaseOfRawMaterial = await _context.PurchaseOfRawMaterials.FindAsync(id);
            //sumOfPurchase = (decimal)purchaseOfRawMaterial.Summa;
            //amounOfPurchase = (decimal)purchaseOfRawMaterial.Amount;
            if (purchaseOfRawMaterials.FirstOrDefault() == null)
            {
                return NotFound();
            }
            //var employe = _context.Posts.FromSqlRaw("dbo.indexPost").ToList();
            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            ViewData["Employee"] = new SelectList(dataEmploye.Where(i=>i.Id==purchaseOfRawMaterials.FirstOrDefault().Employee), "Id", "Names", purchaseOfRawMaterials.FirstOrDefault().Employee);
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial.Where(i=>i.Id==purchaseOfRawMaterials.FirstOrDefault().RawMaterial), "Id", "Title", purchaseOfRawMaterials.FirstOrDefault().RawMaterial);

            return View(purchaseOfRawMaterials.FirstOrDefault());
        }


        //

        // POST: PurchaseOfRawMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseOfRawMaterial purchaseOfRawMaterial)
        {
            if (id != purchaseOfRawMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //DateTime dateNow = DateTime.Now;
                //List<Budget> budgets = await _context.Budgets.ToListAsync();
                //List<RawMaterial> rawMaterials = await _context.RawMaterials.ToListAsync();
                //if (purchaseOfRawMaterial.Summa > budgets[0].SumOfBudget + sumOfPurchase)
                //{
                //    ModelState.AddModelError("Summa", "Not enough budget");
                //}
                //else if (purchaseOfRawMaterial.Summa < 0)
                //{
                //    ModelState.AddModelError("Summa", "The sum cannot be negative");
                //}
                //else if (purchaseOfRawMaterial.Summa == 0)
                //{
                //    ModelState.AddModelError("Summa", "The sum cannot be 0");
                //}
                //else if (purchaseOfRawMaterial.Summa == null)
                //{
                //    ModelState.AddModelError("Summa", "The field cannot be empty");
                //}
                //else if (purchaseOfRawMaterial.Amount == null)
                //{
                //    ModelState.AddModelError("Amount", "The field cannot be empty");
                //}
                //else if (purchaseOfRawMaterial.Amount == 0)
                //{
                //    ModelState.AddModelError("Amount", "The amount cannot be 0");
                //}
                //else if (purchaseOfRawMaterial.Amount < 0)
                //{
                //    ModelState.AddModelError("Amount", "The amount cannot be negative");
                //}
                //else if (purchaseOfRawMaterial.Datee > dateNow)
                //{
                //    ModelState.AddModelError("Datee", "The date is wrong");
                //}
                //else if (purchaseOfRawMaterial.Datee == null)
                //{
                //    ModelState.AddModelError("Datee", "The field cannot be empty");
                //}
                //else
                //{

                //    budgets[0].SumOfBudget += sumOfPurchase;
                //    budgets[0].SumOfBudget -= purchaseOfRawMaterial.Summa;
                //    foreach (var item in rawMaterials)
                //    {
                //        if (item.Id == purchaseOfRawMaterial.RawMaterial)
                //        {
                //            item.Amount -= amounOfPurchase;
                //            item.Amount += purchaseOfRawMaterial.Amount;
                //            item.Summa -= sumOfPurchase;
                //            item.Summa += purchaseOfRawMaterial.Summa;
                //        }
                //    }
                //    sumOfPurchase = 0;
                //    amounOfPurchase = 0;
                //    _context.Update(purchaseOfRawMaterial);
                //    await _context.SaveChangesAsync();
                try
                {
                    var sal = await _context.Salaries.Where(i => i.Issued == true).ToListAsync();

                    foreach (var i in sal)
                    {
                        if (i.Date == purchaseOfRawMaterial.Datee.Value.Year && i.Month == purchaseOfRawMaterial.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(_context.Employees, "Id", "Names");
                            ViewData["RawMaterial"] = new SelectList(_context.RawMaterials, "Id", "Title");
                            return View();
                        }
                        else if(i.Date == p.Datee.Value.Year && i.Month == p.Datee.Value.Month)
                        {
                            ViewBag.Message = "Zp uzhe vydano";
                            ViewData["Employee"] = new SelectList(_context.Employees.Where(i => i.Id == purchaseOfRawMaterial.Employee), "Id", "Names", purchaseOfRawMaterial.Employee);
                            ViewData["RawMaterial"] = new SelectList(_context.RawMaterials.Where(i => i.Id == purchaseOfRawMaterial.RawMaterial), "Id", "Title", purchaseOfRawMaterial.RawMaterial);
                            return View();
                        }
                    }

                    SqlParameter Id = new SqlParameter("@Id", purchaseOfRawMaterial.Id);
                    SqlParameter RawMaterial = new SqlParameter("@RawMaterial", purchaseOfRawMaterial.RawMaterial);
                    SqlParameter Amount = new SqlParameter("@Amount", purchaseOfRawMaterial.Amount);
                    SqlParameter Summa = new SqlParameter("@Summa", purchaseOfRawMaterial.Summa);
                    SqlParameter Datee = new SqlParameter("@Datee", purchaseOfRawMaterial.Datee);
                    SqlParameter Employee = new SqlParameter("@Employee", purchaseOfRawMaterial.Employee);
                    var outParam = new SqlParameter
                    {
                        ParameterName = "@Param",
                        DbType = System.Data.DbType.String,
                        Size = 100,
                        Direction = System.Data.ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editPurchaseOfRawMaterial @Id, @RawMaterial, @Amount, @Summa, @Datee, @Employee, @Param OUT", Id, RawMaterial, Amount, Summa, Datee, Employee, outParam);
                    if (outParam.SqlValue.ToString() == "Null")
                        return RedirectToAction(nameof(Index));

                    if (outParam.SqlValue.ToString() != null)
                    {
                        ViewBag.Message = outParam.SqlValue.ToString();
                        ViewData["Employee"] = new SelectList(_context.Employees.Where(i => i.Id == purchaseOfRawMaterial.Employee), "Id", "Names", purchaseOfRawMaterial.Employee);
                        ViewData["RawMaterial"] = new SelectList(_context.RawMaterials.Where(i => i.Id == purchaseOfRawMaterial.RawMaterial), "Id", "Title", purchaseOfRawMaterial.RawMaterial);
                        return View();
                    }
                }
                catch(SqlException ex)
                {
                    return NotFound(ex.Message);
                }
            }

            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();
            var dataEmploye = _context.Employees.FromSqlRaw("dbo.indexEmploye").ToList();
            ViewData["Employee"] = new SelectList(dataEmploye, "Id", "Names", purchaseOfRawMaterial.Employee);
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", purchaseOfRawMaterial.RawMaterial);

            return View(purchaseOfRawMaterial);
        }

        // GET: PurchaseOfRawMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var purchaseOfRawMaterial = await _context.PurchaseOfRawMaterials
            //    .Include(p => p.EmployeeNavigation)
            //    .Include(p => p.RawMaterialNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var purchaseOfRawMaterials = await _context.PurchaseOfRawMaterials.FromSqlRaw("dbo.selectByIdPurchaseOfRawMaterial @Id", Id).ToListAsync();

            SqlParameter IdRaw = new SqlParameter("@IdRaw", purchaseOfRawMaterials.FirstOrDefault().RawMaterial);
            var rawMaterials = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @IdRaw", IdRaw).ToListAsync();

            SqlParameter IdEmp = new SqlParameter("@IdEmp", purchaseOfRawMaterials.FirstOrDefault().Employee);
            var employees = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @IdEmp", IdEmp).ToListAsync();

            if (purchaseOfRawMaterials.FirstOrDefault().RawMaterial == rawMaterials.FirstOrDefault().Id)
                purchaseOfRawMaterials.FirstOrDefault().RawMaterialNavigation.Title = rawMaterials.FirstOrDefault().Title;
            if (purchaseOfRawMaterials.FirstOrDefault().Employee == employees.FirstOrDefault().Id)
                purchaseOfRawMaterials.FirstOrDefault().EmployeeNavigation.Names = employees.FirstOrDefault().Names;

            if (purchaseOfRawMaterials.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(purchaseOfRawMaterials.FirstOrDefault());
        }

        // POST: PurchaseOfRawMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var purchaseOfRawMaterial = await _context.PurchaseOfRawMaterials.FindAsync(id);
            //List<Budget> budgets = await _context.Budgets.ToListAsync();
            //List<RawMaterial> rawMaterials = await _context.RawMaterials.ToListAsync();
            //List<PurchaseOfRawMaterial> purchaseOfRawMaterials = await _context.PurchaseOfRawMaterials.ToListAsync();
            //foreach(var item in rawMaterials)
            //{
            //    if (item.Id == purchaseOfRawMaterial.RawMaterial)
            //    {
            //        budgets[0].SumOfBudget = budgets[0].SumOfBudget + purchaseOfRawMaterial.Summa;
            //        item.Amount = item.Amount - purchaseOfRawMaterial.Amount;
            //        item.Summa -= purchaseOfRawMaterial.Summa;
            //    }
            //}

            //    _context.PurchaseOfRawMaterials.Remove(purchaseOfRawMaterial);
            //await _context.SaveChangesAsync();
            try
            {
                SqlParameter Id = new SqlParameter("@Id", id);
                await _context.Database.ExecuteSqlRawAsync("exec dbo.deletePurchaseOfRawMaterial @Id", Id);
                return RedirectToAction(nameof(Index));
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private bool PurchaseOfRawMaterialExists(int id)
        {
            return _context.PurchaseOfRawMaterials.Any(e => e.Id == id);
        }
    }
}
