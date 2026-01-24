using System;
using System.Linq;
using WebApplication10.Models;

namespace WebApplication10.Services
{
    public class ResetPasswordMailService
    {
        private readonly TechStoreDBEntities _db;

        public ResetPasswordMailService(TechStoreDBEntities db)
        {
            _db = db;
        }

        public void QueueResetPasswordMail(string email, string baseUrl)
        {
            var token = Guid.NewGuid().ToString("N");

            var oldTokens = _db.PasswordResetTokens
                .Where(x => x.Email == email && x.Used == false);

            _db.PasswordResetTokens.RemoveRange(oldTokens);

            var resetToken = new PasswordResetTokens
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddMinutes(15),
                Used = false
            };

            _db.PasswordResetTokens.Add(resetToken);

            var resetLink = $"{baseUrl}/Account/ResetPassword?token={token}";

            var mail = new EmailQueue
            {
                ToEmail = email,
                Subject = "Đặt lại mật khẩu - Tech Store",
                Body = $@"
                    <p>Bạn đã yêu cầu đặt lại mật khẩu.</p>
                    <p>
                        <a href='{resetLink}'>Nhấn vào đây để đặt lại mật khẩu</a>
                    </p>
                    <p>Liên kết có hiệu lực trong 15 phút.</p>
                ",
                EmailType = "RESET_PASSWORD",
                Status = 0,
                RetryCount = 0,
                CreatedAt = DateTime.Now
            };

            _db.EmailQueue.Add(mail);

            _db.SaveChanges();
        }
    }
}