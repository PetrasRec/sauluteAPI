using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Models
{
    public class BeaconLiveData
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public double RSI { get; set; }
    }
}
