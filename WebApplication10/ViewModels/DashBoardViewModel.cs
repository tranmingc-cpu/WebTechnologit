
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebApplication10.ViewModels
{
    public class DashboardViewModel
    {
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
        public int ProductCount { get; set; }

        public decimal CurrentMonthRevenue { get; set; }

        public int Month { get; set; }
        // Chart
        public List<int> Months { get; set; }
        public List<decimal?> Revenues { get; set; }

    }
}