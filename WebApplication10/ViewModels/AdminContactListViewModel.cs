using System;
using System.Collections.Generic;

namespace WebApplication10.ViewModels
{
    public class AdminContactListViewModel
    {
        public List<ContactItemViewModel> Contacts { get; set; }
    }

    public class ContactItemViewModel
    {
        public int ContactId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}