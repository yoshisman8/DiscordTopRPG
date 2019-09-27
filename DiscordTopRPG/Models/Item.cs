using DiscordTopRPG.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // public Effects Effect { get; set; }
        // public int Value { get; set; }
        // public int EffectSpecific { get; set; }
    }
}
