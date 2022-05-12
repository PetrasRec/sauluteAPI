using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Models
{

    public class Beacon
    {

        [Key]
        public int Id { get; set; }

        public string Identification { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
