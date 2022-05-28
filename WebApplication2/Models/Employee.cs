using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Productions = new HashSet<Production>();
            PurchaseOfRawMaterials = new HashSet<PurchaseOfRawMaterial>();
            Salaries = new HashSet<Salary>();
            SaleOfProducts = new HashSet<SaleOfProduct>();
        }
        public int Id { get; set; }
        public string? Names { get; set; }
        public int? Position { get; set; }
        public decimal? Salary { get; set; }
        public string? Address { get; set; }
        public int? Phone { get; set; }

        public virtual Post? PositionNavigation { get; set; }
        public virtual ICollection<Production> Productions { get; set; }
        public virtual ICollection<PurchaseOfRawMaterial> PurchaseOfRawMaterials { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
        public virtual ICollection<SaleOfProduct> SaleOfProducts { get; set; }
    }
}
