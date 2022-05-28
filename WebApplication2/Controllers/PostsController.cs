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
    public class PostsController : Controller
    {
        private readonly DBaseContext _context;

        public PostsController(DBaseContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(int pg = 1)
        {
            var dataPost = await _context.Posts.FromSqlRaw("dbo.indexPost").ToListAsync();
            //IQueryable<Post> posts = _context.Posts;
            const int pageSize = 7;
            if (pg < 1)
                pg = 1;
            int resCount = dataPost.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<Post> data = dataPost.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var post = await _context.Posts
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter IdPost = new SqlParameter("@IdPost", id);
            var post = await _context.Posts.FromSqlRaw("dbo.selectByIdPost @IdPost", IdPost).ToListAsync();

            if (post.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(post.FirstOrDefault());
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(post);
                    //await _context.SaveChangesAsync();
                    SqlParameter Position = new SqlParameter("@Position", post.Position);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createPost @Position", Position);
                    return RedirectToAction(nameof(Index));
                }
                return View(post);
            }
            catch(SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var post = await _context.Posts.FromSqlRaw("dbo.selectByIdPost @Id", Id).ToListAsync();

            //var post = await _context.Posts.FindAsync(id);
            if (post.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(post.FirstOrDefault());
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(post);
                    //await _context.SaveChangesAsync();
                    SqlParameter Id = new SqlParameter("@Id", post.Id);
                    SqlParameter Position = new SqlParameter("@Position", post.Position);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editPost @Id, @Position", Id, Position);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var post = await _context.Posts
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var post = await _context.Posts.FromSqlRaw("dbo.selectByIdPost @Id", Id).ToListAsync();

            if (post.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(post.FirstOrDefault());
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var post = await _context.Posts.FindAsync(id);
            //_context.Posts.Remove(post);
            //await _context.SaveChangesAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.deletePost @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
