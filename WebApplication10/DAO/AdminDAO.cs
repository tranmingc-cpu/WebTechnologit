using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;
using WebApplication10.ViewModels;

namespace WebApplication10.DAO
{
    public class AdminDao
    {
        private readonly TechStoreDBEntities _context;

        public AdminDao(TechStoreDBEntities context)
        {
            _context = context;
        }

        public int GetUserCount()
        {
            return _context.Users.Count();
        }

        public int GetOrderCount()
        {
            return _context.Orders.Count();
        }
        public int GetProductCount()
        {
            return _context.Products.Count();
        }
        public decimal GetRevenueByMonth(int year, int month)
        {
            return _context.Orders
                .Where(o =>
                    o.OrderDate.HasValue &&
                      o.OrderDate.Value.Year == year &&
                    o.OrderDate.HasValue &&
                      o.OrderDate.Value.Month == month && 
                    (o.Status == "Paid" || o.Status == "Completed")
                )
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;
        }

        public List<DashboardViewModel> GetRevenueByYear(int year)
        {
            return _context.Orders
                .Where(o =>
                      o.OrderDate.HasValue &&
                      o.OrderDate.Value.Year == year &&
                    (o.Status == "Paid" || o.Status == "Completed")
                )
                .GroupBy(o => o.OrderDate.Value.Month)
                .Select(g => new DashboardViewModel
                {
                    Month = g.Key,
                    CurrentMonthRevenue = g.Sum(o => (decimal?)o.TotalAmount) ?? 0 
                })
                .OrderBy(x => x.Month)
                .ToList();
        }
    }

}
