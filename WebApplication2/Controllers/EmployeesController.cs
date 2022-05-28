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
    public class EmployeesController : Controller
    {
        private readonly DBaseContext _context;

        public EmployeesController(DBaseContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(int pg = 1)
        {
            var dataEmploye = await _context.Employees.FromSqlRaw("dbo.indexEmploye").ToListAsync();
            var dataPost = await _context.Posts.FromSqlRaw("dbo.indexPost").ToListAsync();

            //var dBaseContext = _context.Employees.Include(e => e.PositionNavigation);
            //IQueryable<Employee> employees = _context.Employees.Include(p => p.PositionNavigation);
            foreach (var e in dataEmploye)
            {
                foreach (var p in dataPost)
                {
                    if (e.Position == p.Id)
                    {
                        e.PositionNavigation.Position = p.Position;
                    }
                }
            }



            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = dataEmploye.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<Employee> data = dataEmploye.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var employee = await _context.Employees
            //    .Include(e => e.PositionNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var employee = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @Id", Id).ToListAsync();

            SqlParameter IdPost = new SqlParameter("@IdPost", employee[0].Position);
            var post = await _context.Posts.FromSqlRaw("dbo.selectByIdPost @IdPost", IdPost).ToListAsync();

            if (employee.FirstOrDefault().Position == post.FirstOrDefault().Id)
                employee.FirstOrDefault().PositionNavigation.Position = post.FirstOrDefault().Position;
            if (employee.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(employee.FirstOrDefault());
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            var post = _context.Posts.FromSqlRaw("dbo.indexPost").ToList();
            ViewData["Position"] = new SelectList(post, "Id", "Position");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Names = new SqlParameter("@Names", employee.Names);
                    SqlParameter Position = new SqlParameter("@Position", employee.Position);
                    SqlParameter Salary = new SqlParameter("@Salary", employee.Salary);
                    SqlParameter Address = new SqlParameter("@Address", employee.Address);
                    SqlParameter Phone = new SqlParameter("@Phone", employee.Phone);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createEmploye @Names, @Position, @Salary, @Address, @Phone", Names, Position, Salary, Address, Phone);

                    //_context.Add(employee);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                var post = _context.Posts.FromSqlRaw("dbo.indexPost").ToList();
                ViewData["Position"] = new SelectList(post, "Id", "Position", employee.Position);
                return View(employee);
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var employee = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @Id", Id).ToListAsync();
            var dataPost = await _context.Posts.FromSqlRaw("dbo.indexPost").ToListAsync();
            //var employee = await _context.Employees.FindAsync(id);
            if (employee.FirstOrDefault() == null)
            {
                return NotFound();
            }
            ViewData["Position"] = new SelectList(dataPost, "Id", "Position", employee.FirstOrDefault().Position);
            return View(employee.FirstOrDefault());
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try 
                {
                    SqlParameter Id = new SqlParameter("@Id", employee.Id);
                    SqlParameter Names = new SqlParameter("@Names", employee.Names);
                    SqlParameter Position = new SqlParameter("@Position", employee.Position);
                    SqlParameter Salary = new SqlParameter("@Salary", employee.Salary);
                    SqlParameter Address = new SqlParameter("@Address", employee.Address);
                    SqlParameter Phone = new SqlParameter("@Phone", employee.Phone);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editEmploye @Id, @Names, @Position, @Salary, @Address, @Phone",Id, Names, Position, Salary, Address, Phone);

                    //_context.Update(employee);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            var dataPost = await _context.Posts.FromSqlRaw("dbo.indexPost").ToListAsync();
            ViewData["Position"] = new SelectList(_context.Posts, "Id", "Position", employee.Position);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var employee = await _context.Employees.FromSqlRaw("dbo.selectByIdEmploye @Id", Id).ToListAsync();

            SqlParameter IdPost = new SqlParameter("@IdPost", employee.FirstOrDefault().Position);
            var post = await _context.Posts.FromSqlRaw("dbo.selectByIdPost @IdPost", IdPost).ToListAsync();

            //var employee = await _context.Employees
            //    .Include(e => e.PositionNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (employee.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(employee.FirstOrDefault());
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var employee = await _context.Employees.FindAsync(id);
            //_context.Employees.Remove(employee);
            //await _context.SaveChangesAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteEmploye @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
