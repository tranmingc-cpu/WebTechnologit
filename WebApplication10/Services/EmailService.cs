using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using WebApplication10.ViewModels;

namespace WebApplication10.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _enableSsl;

        public EmailService()
        {
            _smtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "";
            _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "";
            _fromEmail = ConfigurationManager.AppSettings["FromEmail"] ?? _smtpUsername;
            _fromName = ConfigurationManager.AppSettings["FromName"] ?? "TechStore";
            _enableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");
        }

        public bool SendOrderConfirmationEmail(OrderEmailViewModel orderInfo)
        {
            try
            {
                string subject = $"Xác nhận đơn hàng #{orderInfo.OrderId} - TechStore";
                string body = GenerateOrderEmailTemplate(orderInfo);

                return SendEmail(orderInfo.CustomerEmail, subject, body);
            }
            catch (Exception ex)
            {
                // Log error (you can add logging here)
                System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_fromEmail, _fromName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.BodyEncoding = Encoding.UTF8;
                    message.SubjectEncoding = Encoding.UTF8;

                    using (var smtp = new SmtpClient(_smtpHost, _smtpPort))
                    {
                        smtp.EnableSsl = _enableSsl;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SMTP Error: {ex.Message}");
                return false;
            }
        }

        private string GenerateOrderEmailTemplate(OrderEmailViewModel order)
        {
            var sb = new StringBuilder();
            
            // Escape HTML cho các giá trị có thể chứa ký tự đặc biệt
            var customerName = System.Net.WebUtility.HtmlEncode(order.CustomerName ?? "");
            var shippingAddress = System.Net.WebUtility.HtmlEncode(order.ShippingAddress ?? "");
            var customerPhone = System.Net.WebUtility.HtmlEncode(order.CustomerPhone ?? "");
            
            sb.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Xác nhận đơn hàng</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f4f4; padding: 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #ff6b6b, #e34b4b); padding: 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>
                                <span style='color: #fff;'>Tech</span><span style='color: #1a1c2a;'>Store</span>
                            </h1>
                        </td>
                    </tr>
                    
                    <!-- Success Icon -->
                    <tr>
                        <td style='padding: 30px; text-align: center;'>
                            <table cellpadding='0' cellspacing='0' align='center' style='margin: 0 auto;'>
                                <tr>
                                    <td style='width: 80px; height: 80px; background-color: #4caf50; border-radius: 50%; text-align: center; vertical-align: middle; line-height: 80px;'>
                                        <span style='color: white; font-size: 40px; font-weight: bold; display: inline-block; vertical-align: middle; line-height: normal;'>✓</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 0 30px;'>
                            <h2 style='color: #333; text-align: center; margin: 0 0 20px 0;'>Đặt hàng thành công!</h2>
                            <p style='color: #666; line-height: 1.6; text-align: center;'>
                                Xin chào <strong>" + customerName + @"</strong>,<br>
                                Cảm ơn bạn đã đặt hàng tại TechStore. Đơn hàng của bạn đã được tiếp nhận và đang được xử lý.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Order Info -->
                    <tr>
                        <td style='padding: 30px;'>
                            <table width='100%' cellpadding='10' cellspacing='0' style='background-color: #f9f9f9; border-radius: 8px;'>
                                <tr>
                                    <td style='border-bottom: 1px solid #e0e0e0;'>
                                        <strong style='color: #333;'>Mã đơn hàng:</strong>
                                    </td>
                                    <td style='text-align: right; border-bottom: 1px solid #e0e0e0;'>
                                        <span style='color: #ff5c5c; font-weight: bold;'>#" + order.OrderId + @"</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='border-bottom: 1px solid #e0e0e0;'>
                                        <strong style='color: #333;'>Ngày đặt:</strong>
                                    </td>
                                    <td style='text-align: right; border-bottom: 1px solid #e0e0e0;'>
                                        <span style='color: #666;'>" + order.OrderDate.ToString("dd/MM/yyyy HH:mm") + @"</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='border-bottom: 1px solid #e0e0e0;'>
                                        <strong style='color: #333;'>Địa chỉ giao hàng:</strong>
                                    </td>
                                    <td style='text-align: right; border-bottom: 1px solid #e0e0e0;'>
                                        <span style='color: #666;'>" + shippingAddress + @"</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong style='color: #333;'>Số điện thoại:</strong>
                                    </td>
                                    <td style='text-align: right;'>
                                        <span style='color: #666;'>" + customerPhone + @"</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Products -->
                    <tr>
                        <td style='padding: 0 30px 30px 30px;'>
                            <h3 style='color: #333; margin: 0 0 15px 0;'>Chi tiết đơn hàng:</h3>
                            <table width='100%' cellpadding='10' cellspacing='0' style='border: 1px solid #e0e0e0; border-radius: 8px;'>
                                <thead>
                                    <tr style='background-color: #f9f9f9;'>
                                        <th style='text-align: left; color: #333; padding: 12px;'>Sản phẩm</th>
                                        <th style='text-align: center; color: #333; padding: 12px;'>SL</th>
                                        <th style='text-align: right; color: #333; padding: 12px;'>Đơn giá</th>
                                        <th style='text-align: right; color: #333; padding: 12px;'>Thành tiền</th>
                                    </tr>
                                </thead>
                                <tbody>");

            foreach (var item in order.OrderItems)
            {
                var productName = System.Net.WebUtility.HtmlEncode(item.ProductName ?? "");
                sb.Append($@"
                                    <tr style='border-bottom: 1px solid #e0e0e0;'>
                                        <td style='padding: 12px; color: #333;'>{productName}</td>
                                        <td style='text-align: center; padding: 12px; color: #666;'>{item.Quantity}</td>
                                        <td style='text-align: right; padding: 12px; color: #666;'>{item.UnitPrice:N0}đ</td>
                                        <td style='text-align: right; padding: 12px; color: #333; font-weight: bold;'>{item.TotalPrice:N0}đ</td>
                                    </tr>");
            }

            sb.Append(@"
                                </tbody>
                                <tfoot>
                                    <tr style='background-color: #f9f9f9;'>
                                        <td colspan='3' style='text-align: right; padding: 15px; font-weight: bold; color: #333;'>
                                            Tổng cộng:
                                        </td>
                                        <td style='text-align: right; padding: 15px; font-weight: bold; color: #ff5c5c; font-size: 18px;'>
                                            " + order.TotalAmount.ToString("N0") + @"đ
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Status -->
                    <tr>
                        <td style='padding: 0 30px 30px 30px;'>
                            <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; border-radius: 4px;'>
                                <p style='margin: 0; color: #856404;'>
                                    <strong>Trạng thái:</strong> Đang chờ xử lý<br>
                                    Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất để xác nhận đơn hàng.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f9f9f9; padding: 20px 30px; text-align: center; border-top: 1px solid #e0e0e0;'>
                            <p style='color: #666; margin: 0 0 10px 0; font-size: 14px;'>
                                Cảm ơn bạn đã tin tưởng và mua sắm tại TechStore!
                            </p>
                            <p style='color: #999; margin: 0; font-size: 12px;'>
                                © 2025 TechStore. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>");

            return sb.ToString();
        }
    }
}
