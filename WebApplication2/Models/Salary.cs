using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class Salary
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int? Month { get; set; }
        public int? Employee { get; set; }
        public int? Purchase { get; set; }
        public int? Sale { get; set; }
        public int? Production { get; set; }
        public int? Count { get; set; }
        public decimal? Salary1 { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? Issued { get; set; }
        public decimal? Bonus { get; set; }

        public virtual Employee? EmployeeNavigation { get; set; }
        public virtual Month? MonthNavigation { get; set; }
    }
}
