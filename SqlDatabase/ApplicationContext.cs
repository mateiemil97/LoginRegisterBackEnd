using System;
using Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SqlDatabase
{
    public class ApplicationContext: IdentityDbContext
    {
        public ApplicationContext(DbContextOptions options): base(options)
        {
            
        }
        
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    }
}
