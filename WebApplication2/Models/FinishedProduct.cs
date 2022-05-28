using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class FinishedProduct
    {
        public FinishedProduct()
        {
            Ingredients = new HashSet<Ingredient>();
            Productions = new HashSet<Production>();
            SaleOfProducts = new HashSet<SaleOfProduct>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Unit { get; set; }
        public decimal? Summa { get; set; }
        public decimal? Amount { get; set; }

        public virtual Unit? UnitNavigation { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<Production> Productions { get; set; }
        public virtual ICollection<SaleOfProduct> SaleOfProducts { get; set; }

        public static implicit operator List<object>(FinishedProduct v)
        {
            throw new NotImplementedException();
        }
    }
}
