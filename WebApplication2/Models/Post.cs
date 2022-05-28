using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class Post
    {
        public Post()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string? Position { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
