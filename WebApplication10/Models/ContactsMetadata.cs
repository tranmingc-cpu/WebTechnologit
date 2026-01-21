using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models
{
    [MetadataType(typeof(ContactsMetadata))]
    public partial class Contacts
    {
    }

    public class ContactsMetadata
    {
        [Required(ErrorMessage = "Họ và tên bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ")]
        public string Message { get; set; }
    }
}
