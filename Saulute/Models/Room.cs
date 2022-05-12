using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public Beacon Beacon { get; set; }
        public double Corner1 { get; set; }
        public double Corner2 { get; set; }
        public double Corner3 { get; set; }
        public double Corner4 { get; set; }
    }
}
