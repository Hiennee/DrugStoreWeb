using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Authentication.Helper
{
    public class InitDB
    {
        public static async void Execute(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                await CreateRoles(roleManager);
                await CreateAdmin(userManager);
            }
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("****: Creating roles...");
            string[] roleNames = { "Admin", "Employee", "Customer" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and store them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            Console.WriteLine("****: Roles already for use");
        }

        private static async Task CreateAdmin(UserManager<IdentityUser> userManager)
        {
            Console.WriteLine("****: Creating the admin user...");
            string email = "admin";
            var admin = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            string pwd = "1";

            var _user = await userManager.FindByEmailAsync(admin.Email);
            if (_user == null)
            {
                var createAdmin = await userManager.CreateAsync(admin, pwd);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                Console.WriteLine("****: Result: " + createAdmin.ToString());
            }
        }
    }
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class SortInfo
    {
        public string? orderBy { get; set; }
        public string? orderType { get; set; }
        public SortInfo(string? orderBy, string? orderType)
        {
            this.orderBy = orderBy;
            this.orderType = orderType;
        }

        public SortInfo()
        {
        }
    }
    public class FilterInfo
    {
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
        public double? FromPrice { get; set; }
        public double? ToPrice { get; set; }
    }
}
