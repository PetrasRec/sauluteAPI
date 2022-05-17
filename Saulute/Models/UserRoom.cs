using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Saulute.Models
{
    public class UserRoom
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int BeaconId { get; set; }
        public int RoomId { get; set; }
        public IdentityUser Owner { get; set; }
        public double Corner1 { get; set; }
        public double Corner2 { get; set; }
        public double Corner3 { get; set; }
        public double Corner4 { get; set; }
    }
}
