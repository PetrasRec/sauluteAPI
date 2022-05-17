using System.ComponentModel.DataAnnotations;
using System;
namespace Saulute.Models
{
    public class RSI
    {
        public int Id { get; set; }
        public int Rsi { get; set; }
        public DateTime Time { get; set; }
        public string IsRequested   { get; set; }
        public int BeaconId { get; set; }
    }
}
