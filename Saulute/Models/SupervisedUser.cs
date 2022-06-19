using Microsoft.AspNetCore.Identity;
using Saulute.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Addicted.Models
{
    public class SupervisedUser
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public IdentityUser Watcher { get; set; }
        public ICollection<UserRoom> Rooms { get; set; }
    }
}
