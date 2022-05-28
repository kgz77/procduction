using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class ReportOfSalesOfProduct
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Datee { get; set; }
        public string? Names { get; set; }
    }
}
