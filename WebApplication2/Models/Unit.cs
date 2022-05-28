using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class Unit
    {
        public Unit()
        {
            FinishedProducts = new HashSet<FinishedProduct>();
            RawMaterials = new HashSet<RawMaterial>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }

        public virtual ICollection<FinishedProduct> FinishedProducts { get; set; }
        public virtual ICollection<RawMaterial> RawMaterials { get; set; }
    }
}
