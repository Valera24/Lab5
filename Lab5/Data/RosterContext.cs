using Lab3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Lab3.Data
{
    public class RosterContext : DbContext
    {
        public RosterContext(DbContextOptions<RosterContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<IdentityUser> IdentityUser { get; set; }
    }
}
