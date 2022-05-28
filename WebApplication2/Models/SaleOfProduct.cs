using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public partial class SaleOfProduct
    {
        public int Id { get; set; }
        public int? Product { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Summa { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Datee { get; set; }
        public int? Employee { get; set; }

        public virtual Employee? EmployeeNavigation { get; set; }
        public virtual FinishedProduct? ProductNavigation { get; set; }
    }
}
