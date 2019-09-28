using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Models
{
    public class PlayerItem
    {
        [Key]
        public int Id { get; set; }
        public Item Item { get; set; }
        public ApplicationUser User { get; set; }
        public int Quantity { get; set; }
    }
}
