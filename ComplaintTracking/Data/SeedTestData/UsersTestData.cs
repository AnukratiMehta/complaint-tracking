﻿using System.Threading.Tasks;
using ComplaintTracking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ComplaintTracking.Data
{
    public partial class SeedTestData
    {
        public static async Task AddUsersAsync(
            ApplicationDbContext _context, UserManager<ApplicationUser> _userManager)
        {
            const string password = "Abc123!!";

            NewUser[] users =
            {
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "DivManager10",
                    Email = "example10@dnr.ga.gov",
                    OfficeName = "Director's Office",
                    CtsRole = CtsRole.DivisionManager,
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Manager11",
                    Email = "example11@dnr.ga.gov",
                    OfficeName = "Mountain District",
                    CtsRole = CtsRole.Manager,
                    IsMaster = true,
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Staff12",
                    Email = "example12@dnr.ga.gov",
                    OfficeName = "Air Protection Branch",
                    IsMaster = true,
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Manager13",
                    Email = "example13@dnr.ga.gov",
                    OfficeName = "Air Protection Branch",
                    CtsRole = CtsRole.Manager,
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Manager1",
                    Email = "example1@dnr.ga.gov",
                    OfficeName = "Mountain District",
                    CtsRole = CtsRole.Manager,
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Staff2",
                    Email = "example2@dnr.ga.gov",
                    OfficeName = "Mountain District",
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Staff3",
                    Email = "example3@dnr.ga.gov",
                    OfficeName = "Mountain District",
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Manager4",
                    Email = "example4@dnr.ga.gov",
                    OfficeName = "Air Protection Branch",
                    CtsRole = CtsRole.Manager,
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Staff5",
                    Email = "example5@dnr.ga.gov",
                    OfficeName = "Air Protection Branch",
                },
                new NewUser
                {
                    FirstName = "Sample",
                    LastName = "Staff6",
                    Email = "example6@dnr.ga.gov",
                    OfficeName = "Air Protection Branch",
                },
                new NewUser
                {
                    FirstName = "Inactive",
                    LastName = "Staff7",
                    Email = "example7@dnr.ga.gov",
                    OfficeName = "Air Protection Branch",
                    Active = false,
                },
            };

            foreach (var u in users)
            {
                if (await _context.Users.AnyAsync(e => e.Email == u.Email)) continue;

                var user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    EmailConfirmed = true,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Active = u.Active,
                    Office = await _context.LookupOffices.FirstOrDefaultAsync(e => e.Name == u.OfficeName),
                };
                await _userManager.CreateAsync(user, password);
                var tempUser = await _userManager.FindByNameAsync(u.Email);
                if (u.CtsRole.HasValue)
                {
                    await _userManager.AddToRoleAsync(tempUser, u.CtsRole.Value.ToString());
                }

                if (!u.IsMaster) continue;

                var office = await _context.LookupOffices.FirstOrDefaultAsync(e => e.Name == u.OfficeName);
                office.MasterUserId = tempUser.Id;
                _context.Update(office);
                await _context.SaveChangesAsync();
            }
        }

        private class NewUser
        {
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string OfficeName { get; init; }
            public string Email { get; init; }
            public CtsRole? CtsRole { get; init; }
            public bool IsMaster { get; init; }
            public bool Active { get; init; } = true;
        }
    }
}
