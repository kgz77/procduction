using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public partial class Month
    {
        public Month()
        {
            Salaries = new HashSet<Salary>();
        }

        public int Id { get; set; }
        public string? Month1 { get; set; }

        public virtual ICollection<Salary> Salaries { get; set; }
    }
}
