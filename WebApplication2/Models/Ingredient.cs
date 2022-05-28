using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class Ingredient
    {
        public int Id { get; set; }
        public int? Product { get; set; }
        public int? RawMaterial { get; set; }
        public decimal? Amount { get; set; }

        public virtual FinishedProduct? ProductNavigation { get; set; }
        public virtual RawMaterial? RawMaterialNavigation { get; set; }
    }
}
