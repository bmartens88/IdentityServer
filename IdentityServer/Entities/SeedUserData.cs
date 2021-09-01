using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Entities
{
    public class SeedUserData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<UserContext>(options =>
                options.UseSqlite(connectionString));

            services.AddIdentity<User, IdentityRole>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireNonAlphanumeric = false;
                }).AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            CreateUser(scope, "John", "Doe", "John Doe's Boulevard 323", "USA",
                Guid.NewGuid().ToString(), "JohnPassword", "Administrator", "john@mail.com");
            CreateUser(scope, "Jane", "Doe", "Jane Doe's Avenue 214", "USA",
                Guid.NewGuid().ToString(), "JanePassword", "Visitor", "jane@mail.com");
        }

        private static void CreateUser(IServiceScope scope, string name, string lastName, string address,
            string country, string id, string password, string role, string email)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = userMgr.FindByNameAsync(email).Result;
            if (user != null) return;
            user = new User
            {
                UserName = email,
                Email = email,
                FirstName = name,
                LastName = lastName,
                Address = address,
                Country = country, Id = id
            };

            var result = userMgr.CreateAsync(user, password).Result;
            CheckResult(result);

            result = userMgr.AddToRoleAsync(user, role).Result;
            CheckResult(result);

            result = userMgr.AddClaimsAsync(user, new[]
            {
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtClaimTypes.Role, role),
                new Claim(JwtClaimTypes.Address, user.Address),
                new Claim("country", user.Country)
            }).Result;
            CheckResult(result);
        }

        private static void CheckResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}