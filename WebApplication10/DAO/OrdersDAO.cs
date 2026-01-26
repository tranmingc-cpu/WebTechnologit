
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebApplication10.Models;

namespace WebApplication10.DAO
{
    public class OrdersDAO
    {
        private readonly TechStoreDBEntities _context;
        public OrdersDAO(TechStoreDBEntities context)
        {
            _context = context;
        }

        public List<Orders> GetAllOrders()
        {
            return _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        // Lấy chi tiết 1 đơn hàng
        public Orders GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.OrderId == id);
        }

        // Cập nhật trạng thái đơn hàng
        public void UpdateStatus(int orderId, string status)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
            }
        }
        public int GetOrderCountThisMonth()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            return _context.Orders
                .Count(o => o.OrderDate.HasValue &&
                      o.OrderDate.Value.Year == year &&
                    o.OrderDate.HasValue &&
                      o.OrderDate.Value.Month == month);
        }
        public int GetOrderCountThisYear()
        {
            int year = DateTime.Now.Year;
            return _context.Orders
                .Count(o => o.OrderDate.HasValue &&
                      o.OrderDate.Value.Year == year);
        }

        public Orders GetByIdWithDetails(int id)
        {
            return _context.Orders
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderId == id);
        }



    }
}

