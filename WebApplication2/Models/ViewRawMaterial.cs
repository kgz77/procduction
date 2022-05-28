using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class ViewRawMaterial
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Unit { get; set; }
        public decimal? Summa { get; set; }
        public decimal? Amount { get; set; }
        public string? Expr1 { get; set; }
    }
}
