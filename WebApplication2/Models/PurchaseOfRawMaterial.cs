using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public partial class PurchaseOfRawMaterial
    {
        public int Id { get; set; }
        public int? RawMaterial { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Summa { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Datee { get; set; }
        public int? Employee { get; set; }

        public virtual Employee? EmployeeNavigation { get; set; }
        public virtual RawMaterial? RawMaterialNavigation { get; set; }
    }
}
