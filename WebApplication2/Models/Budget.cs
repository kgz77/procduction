using System;
using System.Collections.Generic;

namespace WebApplication2.Models
{
    public partial class Budget
    {
        public int Id { get; set; }
        public decimal? SumOfBudget { get; set; }
        public int? PercentageOfPremium { get; set; }
        public int? Bonus { get; set; }
    }
}
