using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.ViewModels
{
    public partial class IndexViewModel
    {
        public IEnumerable<Ingredient> Ingredients { get; set; }
        public SelectList FinishedProduct { get; set; }
        public int? SelectedProduct { get; set; }
        public string ProductName { get; set; }
    }
}
