using Demo.IDP.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;

namespace Demo.IDP
{
    public class SeedUserData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<UserContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<User, IdentityRole>(o => {
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<UserContext>()
              .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider
                    .GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    CreateUser(scope, "Tung", "Nguyen", "John Doe's Boulevard 323", "VietNam",
                        "97a3aa4a-7a89-47f3-9814-74497fb92ccb", "123QWEasd!@#",
                        "Administrator", "tung@mail.com");

                    CreateUser(scope, "Phuong", "Le", "Jane Doe's Avenue 214", "USA",
                        "64aca900-7bc7-4645-b291-38f1b7b5963c", "123QWEasd!@#",
                        "Visitor", "phuong@mail.com");
                }
            }
        }

        /// <summary>
        /// create a service scope that we need to retrieve the UserManager service from the service collection
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="name"></param>
        /// <param name="lastName"></param>
        /// <param name="address"></param>
        /// <param name="country"></param>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <param name="email"></param>
        private static void CreateUser(IServiceScope scope, string name, string lastName,
            string address, string country, string id, string password, string role, string email)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = userMgr.FindByNameAsync(email).Result;
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = name,
                    LastName = lastName,
                    Address = address,
                    Country = country,
                    Id = id
                };
                var result = userMgr.CreateAsync(user, password).Result;
                CheckResult(result);

                result = userMgr.AddToRoleAsync(user, role).Result;
                CheckResult(result);

                result = userMgr.AddClaimsAsync(user, new Claim[]
                {
                    new Claim(JwtClaimTypes.GivenName, user.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, user.LastName),
                    new Claim(JwtClaimTypes.Role, role),
                    new Claim(JwtClaimTypes.Address, user.Address),
                    new Claim("country", user.Country)
                }).Result;
                CheckResult(result);
            }
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
