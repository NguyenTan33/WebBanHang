using System.Security.Claims;
using WebBanHang.Data; // nhớ đúng namespace DbContext
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Services
{
    public class RoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetRole(ClaimsPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated)
                return 0;

            // 👉 Lấy UserId từ login hiện tại
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return 0;

            // 👉 Query SQL (qua EF)
            var roleName = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == userId
                            select r.Name)
                            .FirstOrDefault();

            // 👉 Map sang số
            return roleName switch
            {
                "Admin" => 1,
                "Staff" => 2,
                "User" => 3,
                _ => 0
            };
        }
    }
}
