using System.Linq;

namespace WebApplication10.Models
{
    public partial class Users
    {
        public static int GetUserCount(TechStoreDBEntities context)
        {
            if (context == null)
                return 0;

            return context.Users.Count();
        }
    }
}
