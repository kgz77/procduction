#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    public class IngredientsController : Controller
    {
        private static int i = 0;
        private static int? prevProd;
        private readonly DBaseContext _context;

        public IngredientsController(DBaseContext context)
        {
            _context = context;
        }
        
        // GET: Ingredients
        public async Task<IActionResult> Index(int?finprod, string name, int pg=1)
        {
            //List<FinishedProduct> finishedProducts = await _context.FinishedProducts.ToListAsync();
            var finishedProducts = await _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToListAsync();
            var dataRawMaterial = await _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToListAsync();
            var ingredients = await _context.Ingredients.FromSqlRaw("dbo.indexIngredient").ToListAsync();

            finishedProducts.Insert(0, new FinishedProduct { Title = "Все", Id = 0 });
            //IQueryable<Ingredient> ingredients = _context.Ingredients.Include(p => p.ProductNavigation).Include(u => u.RawMaterialNavigation);
            foreach(var ingr in ingredients)
            {
                foreach(var fin in finishedProducts)
                {
                    if(ingr.Product == fin.Id)
                    {
                        ingr.ProductNavigation.Id = fin.Id;
                        ingr.ProductNavigation.Title = fin.Title;
                    }
                }
                foreach(var raw in dataRawMaterial)
                {
                    if (ingr.RawMaterial == raw.Id)
                    {
                        ingr.RawMaterialNavigation.Id = raw.Id;
                        ingr.RawMaterialNavigation.Title = raw.Title;
                    }
                }
            }
            if(finprod!=null && finprod != 0)
            {
                ingredients = ingredients.Where(p => p.ProductNavigation.Id == finprod).ToList();
            }
            if (finprod == null)
            {
                finprod = prevProd;
                ingredients = ingredients.Where(p => p.ProductNavigation.Id == finprod).ToList();
                if(ingredients.Count() == 0)
                {
                    ingredients = await _context.Ingredients.FromSqlRaw("dbo.indexIngredient").ToListAsync();
                    foreach (var ingr in ingredients)
                    {
                        foreach (var fin in finishedProducts)
                        {
                            if (ingr.Product == fin.Id)
                            {
                                ingr.ProductNavigation.Id = fin.Id;
                                ingr.ProductNavigation.Title = fin.Title;
                            }
                        }
                        foreach (var raw in dataRawMaterial)
                        {
                            if (ingr.RawMaterial == raw.Id)
                            {
                                ingr.RawMaterialNavigation.Id = raw.Id;
                                ingr.RawMaterialNavigation.Title = raw.Title;
                            }
                        }
                    }
                }
            }

            const int pageSize = 6;
            if (pg < 1)
                pg = 1;
            int resCount = ingredients.Count();
            var pager = new Pager(resCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = ingredients.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            
            IndexViewModel indexViewModel = new IndexViewModel
            {
                Ingredients = data,
                FinishedProduct = new SelectList(finishedProducts, "Id", "Title"),
                ProductName = name,
                SelectedProduct = finprod
            };

            if (finprod.HasValue)
            {
                var itemToSelect = indexViewModel.FinishedProduct.FirstOrDefault(x => x.Value == finprod.Value.ToString());
                itemToSelect.Selected = true;
            }
            prevProd = finprod;
            //List<Ingredient> ingredientsList = _context.Ingredients.ToList();
            //ViewBag.ingredientsList = new SelectList(ingredientsList, "Product");
            return View(indexViewModel);
        } 

        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var ingredient = await _context.Ingredients
            //    .Include(i => i.ProductNavigation)
            //    .Include(i => i.RawMaterialNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var ingredient = await _context.Ingredients.FromSqlRaw("dbo.selectByIdIngredient @Id", Id).ToListAsync();

            SqlParameter IdProduc = new SqlParameter("@IdProduc", ingredient.FirstOrDefault().Product);
            var product = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @IdProduc", IdProduc).ToListAsync();
            
            SqlParameter IdRaw = new SqlParameter("@IdRaw", ingredient.FirstOrDefault().RawMaterial);
            var raw = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @IdRaw", IdRaw).ToListAsync();

            if (ingredient.FirstOrDefault().Product == product.FirstOrDefault().Id)
                ingredient.FirstOrDefault().ProductNavigation.Title = product.FirstOrDefault().Title;
            if (ingredient.FirstOrDefault().RawMaterial == raw.FirstOrDefault().Id)
                ingredient.FirstOrDefault().RawMaterialNavigation.Title = raw.FirstOrDefault().Title;
            if (ingredient.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(ingredient.FirstOrDefault());
        }

        // GET: Ingredients/Create
        public IActionResult Create()
        {
            var finishedProducts = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();

            ViewData["Product"] = new SelectList(finishedProducts, "Id", "Title");
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(ingredient);
                    //await _context.SaveChangesAsync();
                    SqlParameter Product = new SqlParameter("@Product", ingredient.Product);
                    SqlParameter RawMaterial = new SqlParameter("@RawMaterial", ingredient.RawMaterial);
                    SqlParameter Amount = new SqlParameter("@Amount", ingredient.Amount);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.createIngridient @Product, @RawMaterial, @Amount", Product, RawMaterial, Amount);

                    return RedirectToAction(nameof(Index));
                }
                var finishedProducts = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
                var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();

                ViewData["Product"] = new SelectList(finishedProducts, "Id", "Title", ingredient.Product);
                ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", ingredient.RawMaterial);
                return View(ingredient);
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var ingredient = await _context.Ingredients.FromSqlRaw("dbo.selectByIdIngredient @Id", Id).ToListAsync();

            //var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient.FirstOrDefault() == null)
            {
                return NotFound();
            }
            var finishedProducts = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();

            ViewData["Product"] = new SelectList(finishedProducts, "Id", "Title", ingredient.FirstOrDefault().Product);
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", ingredient.FirstOrDefault().RawMaterial);
            return View(ingredient.FirstOrDefault());
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(ingredient);
                    //await _context.SaveChangesAsync();
                    SqlParameter Id = new SqlParameter("@Id", ingredient.Id);
                    SqlParameter Product = new SqlParameter("@Product", ingredient.Product);
                    SqlParameter RawMaterial = new SqlParameter("@RawMaterial", ingredient.RawMaterial);
                    SqlParameter Amount = new SqlParameter("@Amount", ingredient.Amount);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.editIngridient @Id, @Product, @RawMaterial, @Amount", Id, Product, RawMaterial, Amount);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.Id))
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
            var finishedProducts = _context.FinishedProducts.FromSqlRaw("dbo.indexFinishedProduct").ToList();
            var dataRawMaterial = _context.RawMaterials.FromSqlRaw("dbo.indexRawMaterial").ToList();

            ViewData["Product"] = new SelectList(finishedProducts, "Id", "Title", ingredient.Product);
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", ingredient.RawMaterial);
            return View(ingredient);
        }

        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var ingredient = await _context.Ingredients
            //    .Include(i => i.ProductNavigation)
            //    .Include(i => i.RawMaterialNavigation)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            SqlParameter Id = new SqlParameter("@Id", id);
            var ingredient = await _context.Ingredients.FromSqlRaw("dbo.selectByIdIngredient @Id", Id).ToListAsync();

            SqlParameter IdProduc = new SqlParameter("@IdProduc", ingredient.FirstOrDefault().Product);
            var product = await _context.FinishedProducts.FromSqlRaw("dbo.selectByIdFinishedProduct @IdProduc", IdProduc).ToListAsync();

            SqlParameter IdRaw = new SqlParameter("@IdRaw", ingredient.FirstOrDefault().RawMaterial);
            var raw = await _context.RawMaterials.FromSqlRaw("dbo.selectByIdRawMaterial @IdRaw", IdRaw).ToListAsync();

            if (ingredient.FirstOrDefault().Product == product.FirstOrDefault().Id)
                ingredient.FirstOrDefault().ProductNavigation.Title = product.FirstOrDefault().Title;
            if (ingredient.FirstOrDefault().RawMaterial == raw.FirstOrDefault().Id)
                ingredient.FirstOrDefault().RawMaterialNavigation.Title = raw.FirstOrDefault().Title;

            if (ingredient.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(ingredient.FirstOrDefault());
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var ingredient = await _context.Ingredients.FindAsync(id);
            //_context.Ingredients.Remove(ingredient);
            //await _context.SaveChangesAsync();
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.deleteIngridient @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
}
