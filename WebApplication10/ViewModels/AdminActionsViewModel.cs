using System.Collections.Generic;
using WebApplication10.Models;

namespace WebApplication10.ViewModels
{
    public class AdminActionsViewModel
    {
        public InfoPages About { get; set; }
        public InfoPages Contact { get; set; }
        public InfoPages Warranty { get; set; }
        public InfoPages News { get; set; }
        public InfoPages Careers { get; set; }
        public InfoPages Returns { get; set; }
        public InfoPages Shipping { get; set; }
        public InfoPages Payment { get; set; }
        public List<InfoPages> OtherPages { get; set; } = new List<InfoPages>();

  

    }
}
