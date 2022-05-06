using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saulute.Models
{
    public class BeaconLiveData
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public double RSI { get; set; }
    }
}
