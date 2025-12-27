using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;

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
    }
}
