using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saulute.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    public class AuthenticationContext : IdentityDbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Beacon>()
                .HasMany(b => b.Rooms)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SupervisedUser>()
              .HasMany(b => b.Rooms)
              .WithOne()
              .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<SupervisedUser> SupervisedUsers { get; set; }

        public DbSet<Beacon> Beacons { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public DbSet<UserBeacon> UserBeacons { get; set; }

        public DbSet<UserRoom> UserRooms { get; set; }

        public List<User> GetAllUsers()
        {
            return Users.ToList();
        }
        public User GetUserByEmail(string email)
        {
            return Users.Single(u => u.NormalizedEmail == email.ToUpper());
        }
    }
}
