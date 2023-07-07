using Application.Interface;
using Application.Mappings.AutoMapper;
using Application.Services;
using AutoMapper;
using Core.DataAccess.EntityFramework.Repository;
using DataAccess.EntityFramework;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Entities.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApp
{
    public class Startup
    {
        //public IConfiguration Configuration { get; }
        public IConfiguration configuration { get; }
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration _configuration, IWebHostEnvironment env)
        {
            configuration = _configuration;
            _env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opt => {
            opt.Cookie.Name = "Cookie";
            //opt.Cookie.HttpOnly = true;
            //opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            //opt.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            opt.ExpireTimeSpan = TimeSpan.FromSeconds(99999);
                opt.Cookie.IsEssential = true;
                opt.ReturnUrlParameter = "/Account/SignIn";
                opt.LogoutPath = "/Account/SignIn";
                opt.LoginPath = "/Account/SignIn";
                opt.AccessDeniedPath= "/Account/SignIn";
            });


            //services.Configure<IdentityOptions>(opt =>
            //{
            //    opt.SignIn .ApplicationCookie.LoginPath = new PathString("/aa");
            //    opt.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/InactiveSponsor");
            //    opt.Cookies.ApplicationCookie.LogoutPath = new PathString("/");
            //});
            //.AddCookie(options =>
            // {
            //     options.LoginPath = "/login";
            //     options.LogoutPath = "/logout";
            // });

            //services.AddIdentity<AppUser, AppRole>(opt => opt.Lockout.MaxFailedAccessAttempts = 10).AddEntityFrameworkStores<ET_StoreARP>();
            services.AddRazorPages();
       
            //services.AddMvc().AddRazorRuntimeCompilation();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddHttpContextAccessor();
            services.AddSession();
            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/SignIn");
            var configuration = new MapperConfiguration(opt =>
            {
                opt.AddProfile(new ProductProfile());
                
            });
            // services.AddIdentityCore();
            // signInManager.CheckPasswordSignInAsync()
            // signInManager.PasswordSignInAsync()
            //signInManager.PasswordSignInAsync()
            var mapper = configuration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(99999);
                //options.Cookie.HttpOnly = true;
                //options.Cookie.IsEssential = true;
            });

            //services.Configure<FormOptions>(options =>
            //{
            //    // Set the limit to 128 MB
            //    options.MultipartBodyLengthLimit = 134217728;
            //    options.ValueLengthLimit = 134217728;
            //    options.BufferBodyLengthLimit = 134217728;
            //    options.m
            //});
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 100000000;
                options.ValueLengthLimit = 134217728;
                options.MultipartHeadersCountLimit = 1999129;
                options.MultipartHeadersLengthLimit = 1241121;
            });


        }
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    if (_env.IsDevelopment())
        //    {
        //        Console.WriteLine(_env.EnvironmentName);
        //    }
        //    else if (_env.IsStaging())
        //    {
        //        Console.WriteLine(_env.EnvironmentName);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Not dev or staging");
        //    }

        //    services.AddRazorPages();
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsStaging())
            {
             
            }
            else
           {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions() { FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot").Replace("\\", "/")) });
            //app.UseCookiePolicy(options =>
            //{
            //    options.AutomaticAuthenticate = true;
            //    options.AutomaticChallenge = true;
            //});
            app.UseRouting();
            app.UseSession();
        
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller}/{action}"
               );
                endpoints.MapControllerRoute(
                    name: "default",
                    //pattern: "{controller=Home}/{action=Index1}/{id?}");
                    pattern: "{controller=Account}/{action=SignIn}/{id?}");
            });
        }
    }
}

