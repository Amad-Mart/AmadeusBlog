using AmadeusBlog.Data;
using AmadeusBlog.Enums;
using AmadeusBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Services
{
    public class DataService
    {
        readonly ApplicationDbContext _context;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly IBlogFileService _fileService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;

        public DataService(
            ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager, 
            IBlogFileService fileService, 
            UserManager<BlogUser> userManager, 
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _fileService = fileService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task ManageDataAsync()
        {
            //Task 0: Make sure the DB is present by running through the migrations
            await _context.Database.MigrateAsync();

            //Task 1: Seed Roles - Creating Roles and entering them into the system (AspNetRoles)
            await SeedRolesAsync();

            //Task 2: Seed a few users in the system (AspNetUsers)
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            //Are there any roles already in the system
            if (_context.Roles.Any())
                return;

            foreach (var stringRole in Enum.GetNames(typeof(BlogRoles)))
            {
                var identityRole = new IdentityRole(stringRole);
                await _roleManager.CreateAsync(identityRole);
            }
        }

        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any(u => u.Email == "justjohntester@gmail.com"))
            {
                return;
            }

            var adminUser = new BlogUser()
            {
                Email = "justjohntester@gmail.com",
                UserName = "justjohntester@gmail.com",
                FirstName = "Data",
                LastName = "Dumper",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                ImageData = await _fileService.EncodeFileAsync("DefaultAdminImage.jpg"),
                ContentType = "jpg"
            };

            //await _userManager.CreateAsync(adminUser, "Abc&123!");
            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);

            //await _userManager.AddToRoleAsync(adminUser, "Administrator");
            await _userManager.AddToRoleAsync(adminUser, BlogRoles.Administrator.ToString());

        }

    }
}
