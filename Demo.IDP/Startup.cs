// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using Demo.EmailService;
using Demo.IDP.CustomTokenProviders;
using Demo.IDP.Entities;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Demo.IDP
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; set; }
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();

            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            // register the UserContext class into the service collection.
            services.AddDbContext<UserContext>(options => options
                    .UseSqlServer(Configuration.GetConnectionString("identitySqlConnection")));

            // register ASP.NET Core Identity with a specific user and role classes
            // additionally, we register the Entity Framework Core for Identity and provide a default token provider
            services.AddIdentity<User, IdentityRole>(opt =>
                    {
                        opt.Password.RequireDigit = false;
                        opt.Password.RequiredLength = 7;
                        opt.Password.RequireUppercase = false;
                        opt.User.RequireUniqueEmail = true;
                        opt.SignIn.RequireConfirmedEmail = true;
                        opt.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

                        // setting user lockout
                        //set the lockout period to two minutes (default is five) and maximum failed login attempts to three (default is five)
                        opt.Lockout.AllowedForNewUsers = true;
                        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                        opt.Lockout.MaxFailedAccessAttempts = 3;
                    })
                    .AddEntityFrameworkStores<UserContext>()
                    .AddDefaultTokenProviders()
                    .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation"); //add email custom provie token


            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                //.AddInMemoryIdentityResources(Config.IdentityResources)
                //.AddInMemoryApiScopes(Config.ApiScopes)
                //.AddInMemoryApiResources(Config.Apis)
                //.AddInMemoryClients(Config.Clients)
                //.AddTestUsers(TestUsers.Users)
                // The first context class for the configuration of clients, resources and scopes
                .AddConfigurationStore(opt =>
                 {
                     opt.ConfigureDbContext = c => c.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
                     sql => sql.MigrationsAssembly(migrationAssembly));
                 })
                // The second context class is used for temporary operational data like authorization codes and refresh tokens
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = o => o.UseSqlServer(Configuration.GetConnectionString("sqlConnection"),
                    sql => sql.MigrationsAssembly(migrationAssembly));
                })
                // we add an integration layer to allow IdentityServer to access user data from the ASP.NET Core Identity user database
                .AddAspNetIdentity<User>();

            // not recommended for production - you need to store your key material somewhere secure
            // method which sets temporary signing credentials
            builder.AddDeveloperSigningCredential();

            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            // Add life time email provider token
            services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromDays(3));
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            // we enable serving static files from the wwwroot folder
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
