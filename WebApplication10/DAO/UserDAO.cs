using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication10.Models;
using System.Data.Entity;

namespace WebApplication10.DAO
{
    public class UserDao
    {
        private readonly TechStoreDBEntities _context;

        public UserDao(TechStoreDBEntities context)
        {
            _context = context;
        }

        public IEnumerable<Users> GetAll()
        {
            return _context.Users.ToList();
        }

        public Users GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void Add(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "Customer";

            user.IsActive = true;
            user.CreatedAt = DateTime.Now;

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var existingUser = _context.Users.Find(user.UserId);
            if (existingUser == null)
                throw new InvalidOperationException("User not found.");

            if (!string.IsNullOrWhiteSpace(user.FullName))
                existingUser.FullName = user.FullName;

            existingUser.Phone = user.Phone;
            existingUser.Address = user.Address;

            if (!string.IsNullOrWhiteSpace(user.Username))
                existingUser.Username = user.Username;
            if (!string.IsNullOrWhiteSpace(user.Email))
                existingUser.Email = user.Email;
            if (!string.IsNullOrWhiteSpace(user.Password))
                existingUser.Password = user.Password;

            existingUser.Role = string.IsNullOrWhiteSpace(user.Role) ? existingUser.Role : user.Role;

            _context.SaveChanges();
        }

        public bool Delete(int id, out string message)
        {
            message = null;

            var user = _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Orders)
                .Include(u => u.Reviews)
                .Include(u => u.NewsletterSubscribers)
                .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                message = "Người dùng không tồn tại.";
                return false;
            }

            if (user.Orders.Any())
            {
                message = "Không thể xóa vì người dùng đang có đơn hàng.";
                return false;
            }

            if (user.Cart.Any())
            {
                message = "Không thể xóa vì người dùng đang có giỏ hàng.";
                return false;
            }

            if (user.Reviews.Any())
            {
                message = "Không thể xóa vì người dùng đã có đánh giá.";
                return false;
            }

            if (user.NewsletterSubscribers.Any())
            {
                message = "Không thể xóa vì người dùng đã đăng ký nhận bản tin.";
                return false;
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return true;
        }

        public bool Exists(int id)
        {
            return _context.Users.Any(u => u.UserId == id);
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _context.Users.Find(userId);
            if (user != null && user.Password == oldPassword)
            {
                user.Password = newPassword;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool ExistsByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return _context.Users.Any(u => u.Email == email);
        }

        public bool ExistsByEmail(string email, int excludeUserId)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return _context.Users.Any(u => u.Email == email && u.UserId != excludeUserId);
        }


    }
}
