using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saulute.Models
{
    public class UserBeacon
    {
        [Key]
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public int BeaconId { get; set; }
    }
}
