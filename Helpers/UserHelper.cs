using Demo.Data;

namespace Demo.Helpers
{
    public class UserHelper
    {
        private readonly DataContext _context;

        public UserHelper(DataContext context)
        {
            _context = context;
        }

        public static string GetFullName(long userId)
        {
            return GetFullName(userId);
        }

        public static string GetFullName(long userId, DataContext _context)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            return user?.FullName;
        }
    }
}
