using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Models
{

    public class Beacon
    {
        [Column(TypeName = "nvarchar(255)")]
        public string Identification { get; set; }
        public string Id { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
