using System.ComponentModel.DataAnnotations;

namespace WebApplication10.ViewModels
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
