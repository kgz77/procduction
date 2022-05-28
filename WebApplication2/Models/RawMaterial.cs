using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class RawMaterial
    {
        public RawMaterial()
        {
            Ingredients = new HashSet<Ingredient>();
            PurchaseOfRawMaterials = new HashSet<PurchaseOfRawMaterial>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Unit { get; set; }
        public decimal? Summa { get; set; }
        public decimal? Amount { get; set; }

        public virtual Unit? UnitNavigation { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<PurchaseOfRawMaterial> PurchaseOfRawMaterials { get; set; }
    }
}
